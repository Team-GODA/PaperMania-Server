using Google.Apis.Auth;
using Server.Application.Exceptions;
using Server.Application.Port;
using Server.Domain.Entity;

namespace Server.Infrastructure.Service;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _repository;
    private readonly ISessionService _sessionService;
    private readonly ILogger<AccountService> _logger;

    private readonly string _googleClientId =
        "1072960220447-ifi2uau290btfudnu2ol5b82eq1ucp96.apps.googleusercontent.com";

    public AccountService(IAccountRepository repository, ISessionService sessionService, ILogger<AccountService> logger)
    {
        _repository = repository;
        _sessionService = sessionService;
        _logger = logger;
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
            throw new DuplicateEmailException(player.Email);

        var existByPlayerId = await _repository.GetAccountDataByPlayerIdAsync(player.PlayerId);
        if (existByPlayerId != null)
            throw new DuplicatePlayerIdException(player.PlayerId);
        
        player.Password = BCrypt.Net.BCrypt.HashPassword(password);
        player.IsNewAccount = true;
        player.Role = "user";
        
        var createdPlayer = await _repository.AddAccountAsync(player);
        return createdPlayer;
    }

    public async Task<(string sessionId, PlayerAccountData user)> LoginAsync(string playerId, string password)
    {
        var user = await _repository.GetAccountDataByPlayerIdAsync(playerId);

        if (user == null)
            throw new AuthenticationFailedException("사용자가 존재하지 않습니다.");
        
        bool isVerified = BCrypt.Net.BCrypt.Verify(password, user.Password);
        if (!isVerified)
            throw new AuthenticationFailedException("비밀번호가 일치하지 않습니다.");

        var sessionId = await _sessionService.CreateSessionAsync(user.Id);
        return (sessionId, user);
    }

    public async Task<bool> LogoutAsync(string sessionId)
    {
        var userId = await _sessionService.GetUserIdBySessionIdAsync(sessionId);
        
        var isVaild = await _sessionService.ValidateSessionAsync(sessionId, userId);
        if (!isVaild)
            return false;
        
        await _sessionService.DeleteSessionAsync(sessionId);
        return true;
    }
    
    public async Task<string?> LoginByGoogleAsync(string idToken)
    {
        try
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken,
                new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { _googleClientId }
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
            
            var sessionId = await _sessionService.CreateSessionAsync(user.Id);
            return sessionId;
        }
        catch (InvalidJwtException ex)
        {
            _logger.LogWarning("구글 토큰 검증 실패: {Message}", ex.Message);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError("구글 토큰 검증 중 예기치 못한 오류 발생: {Message}", ex.Message);
            return null;
        }
    }
}