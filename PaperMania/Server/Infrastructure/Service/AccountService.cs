using Google.Apis.Auth;
using Microsoft.Extensions.Options;
using Server.Api.Dto.Response;
using Server.Application.Configure;
using Server.Application.Exceptions;
using Server.Application.Port;
using Server.Domain.Entity;

namespace Server.Infrastructure.Service;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _repository;
    private readonly ISessionService _sessionService;
    private readonly ILogger<AccountService> _logger;
    
    private readonly string _googleClientId;

    public AccountService(IAccountRepository repository, ISessionService sessionService,
        ILogger<AccountService> logger, IOptions<GoogleAuthSetting> googleAuthOptions)
    {
        _repository = repository;
        _sessionService = sessionService;
        _logger = logger;
        _googleClientId = googleAuthOptions.Value.ClientId;
    }
    
    public async Task<PlayerAccountData?> GetByPlayerIdAsync(string playerId)
    {
        return await _repository.GetAccountDataByPlayerIdAsync(playerId);
    }

    public async Task<PlayerAccountData?> GetByEmailAsync(string email)
    {
        return await _repository.GetAccountDataByEmailAsync(email);
    }

    public async Task<PlayerAccountData?> RegisterAsync(PlayerAccountData player, string password)
    {
        var existByEmail = await _repository.GetAccountDataByEmailAsync(player.Email);
        if (existByEmail != null)
            throw new RequestException(ErrorStatusCode.Conflict, "DUPLICATE_EMAIL", new { PlayerId = player.PlayerId, Email = existByEmail.Email });

        var existByPlayerId = await _repository.GetAccountDataByPlayerIdAsync(player.PlayerId);
        if (existByPlayerId != null)
            throw new RequestException(ErrorStatusCode.Conflict, "DUPLICATE_PLAYER_ID", new { PlayerId = player.PlayerId });
        
        player.Password = BCrypt.Net.BCrypt.HashPassword(password);
        player.IsNewAccount = true;
        player.Role = "user";
        
        var createdPlayer = await _repository.AddAccountAsync(player);
        return createdPlayer;
    }

    public async Task<(string sessionId, PlayerAccountData user)> LoginAsync(string playerId, string password)
    {
        var user = await _repository.GetAccountDataByPlayerIdAsync(playerId);
        _logger.LogInformation("UserId : {UserId}", user?.Id.ToString() ?? "null");
        _logger.LogInformation("PlayerId : {PlayerId}", user?.PlayerId ?? "null");

        if (user == null)
            throw new RequestException(ErrorStatusCode.NotFound, "USER_NOT_FOUND", new { PlayerId = playerId, Password = password });
        
        bool isVerified = BCrypt.Net.BCrypt.Verify(password, user.Password);
        if (!isVerified)
            throw new RequestException(ErrorStatusCode.Conflict, "PASSWORD_NOT_VERIFIED", new { PlayerId = playerId, Password = password });

        var sessionId = await _sessionService.CreateSessionAsync(user.Id);
        return (sessionId, user);
    }

    public async Task LogoutAsync(string sessionId)
    {
        await _sessionService.DeleteSessionAsync(sessionId);
    }
    
    public async Task<string?> LoginByGoogleAsync(string idToken)
    {
        try
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken,
                new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = [_googleClientId]
                });
            
            var user = await _repository.GetAccountDataByEmailAsync(payload.Email);
            if (user == null)
            {
                _logger.LogInformation($"신규 구글 사용자 생성: {payload.Email}");

                user = await _repository.AddAccountAsync(new PlayerAccountData
                {
                    PlayerId = payload.Subject,
                    Password = "",
                    Role = "user",
                    Email = payload.Email,
                    IsNewAccount = true,
                    CreatedAt = DateTime.UtcNow
                });
            }
            
            var sessionId = await _sessionService.CreateSessionAsync(user!.Id);
            return sessionId;
        }
        catch (InvalidJwtException ex)
        {
            _logger.LogWarning("구글 토큰 검증 실패: {Message}", ex.Message);
            throw new RequestException(ErrorStatusCode.Conflict, "INVALID_GOOGLE_TOKEN");
        }
    }

    public async Task ValidateUserBySessionIdAsync(string sessionId)
    {
        if (!await _sessionService.ValidateAndRefreshSessionAsync(sessionId))
            throw new RequestException(ErrorStatusCode.Unauthorized, "INVALID_SESSION");
    }
}