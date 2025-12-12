using System.Diagnostics;
using System.Text.Json;
using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port;
using Server.Application.UseCase.Auth.Command;
using Server.Application.UseCase.Auth.Result;
using Server.Domain.Entity;
using Server.Infrastructure.Cache;

namespace Server.Application.UseCase.Auth;

public class LoginService : ILoginUseCase
{
    private readonly IAccountRepository _repository;
    private readonly ISessionService _sessionService;
    private readonly ICacheService _cacheService;

    public LoginService(
        IAccountRepository accountRepository,
        ISessionService sessionService,
        ICacheService cacheService)
    {
        _repository = accountRepository;
        _sessionService = sessionService;
        _cacheService = cacheService;
    }

    public async Task<LoginResult> ExecuteAsync(LoginCommand request)
    {
        VaildateInput(request);

        var cached = await _cacheService.GetAsync(CacheKey.Player.AccountByPlayerId(request.PlayerId));

        PlayerAccountData? account;
        if (cached != null)
        {
            account = JsonSerializer.Deserialize<PlayerAccountData>(cached);
        }
        else
        {
            account = await _repository.FindByPlayerIdAsync(request.PlayerId);

            await _cacheService.SetAsync(CacheKey.Player.AccountByPlayerId(account!.PlayerId),
                JsonSerializer.Serialize(account), TimeSpan.FromDays(30));
        }

        if (account == null || string.IsNullOrEmpty(account.Password))
        {
            BCrypt.Net.BCrypt.Verify(request.Password, 
                BCrypt.Net.BCrypt.HashPassword("dummy", 10));

            throw new RequestException(
                ErrorStatusCode.Unauthorized,
                "INVALID_CREDENTIALS");
        }

        var isVerified = BCrypt.Net.BCrypt.Verify(request.Password, account.Password);

        if (!isVerified)
            throw new RequestException(
                ErrorStatusCode.Unauthorized,
                "INVALID_CREDENTIALS");

        var sessionId = await _sessionService.CreateSessionAsync(account.Id);

        if (string.IsNullOrEmpty(sessionId))
            throw new RequestException(
                ErrorStatusCode.ServerError,
                "SESSION_CREATE_FAILED");

        return new LoginResult(
            SessionId: sessionId,
            IsNewAccount: account.IsNewAccount
        );
    }

    private void VaildateInput(LoginCommand request)
    {
        if (string.IsNullOrWhiteSpace(request.PlayerId))
            throw new RequestException(
                ErrorStatusCode.BadRequest,
                "INVALID_PLAYER_ID");

        if (string.IsNullOrWhiteSpace(request.Password))
            throw new RequestException(
                ErrorStatusCode.BadRequest,
                "INVALID_PASSWORD");
    }
}
