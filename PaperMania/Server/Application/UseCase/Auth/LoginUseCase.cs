using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port;
using Server.Application.UseCase.Auth.Command;
using Server.Application.UseCase.Auth.Result;
using Server.Domain.Service;
using Server.Infrastructure.Cache;

namespace Server.Application.UseCase.Auth;

public class LoginUseCase : ILoginUseCase
{
    private readonly IAccountRepository _repository;
    private readonly UserService _userService;
    private readonly ISessionService _sessionService;
    private readonly CacheWrapper _cache;

    public LoginUseCase(
        IAccountRepository accountRepository,
        UserService userService,
        ISessionService sessionService,
        CacheWrapper cache)
    {
        _repository = accountRepository;
        _userService = userService;
        _sessionService = sessionService;
        _cache = cache;
    }

    public async Task<LoginResult> ExecuteAsync(LoginCommand request)
    {
        ValidateInput(request);

        var account = await _cache.FetchAsync(
            key: CacheKey.Account.ByPlayerId(request.PlayerId),
            fetchFunc: () => _repository.FindByPlayerIdAsync(request.PlayerId),
            expiration: TimeSpan.FromDays(30)
            );

        if (account == null || string.IsNullOrEmpty(account.Password))
        {
            _userService.VerifyPassword(request.Password, "DUMMY_HASH");
            throw new RequestException(
                ErrorStatusCode.Unauthorized,
                "INVALID_CREDENTIALS");
        }

        var isVerified = _userService.VerifyPassword(request.Password, account.Password);
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

    private static void ValidateInput(LoginCommand request)
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
