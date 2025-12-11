using System.Security.Cryptography;
using System.Text;
using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port;
using Server.Application.UseCase.Auth.Command;
using Server.Application.UseCase.Auth.Result;

namespace Server.Application.UseCase.Auth;

public class LoginService : ILoginUseCase
{
    private readonly IAccountRepository _accountRepository;
    private readonly ISessionService _sessionService;

    private static readonly string s_dummyPassword =
        "$2a$11$dummyhashfordatabasedummyhashfordatabase";

    public LoginService(
        IAccountRepository accountRepository,
        ISessionService sessionService)
    {
        _accountRepository = accountRepository;
        _sessionService = sessionService;
    }

    public async Task<LoginResult> ExecuteAsync(LoginCommand request)
    {
        if (string.IsNullOrWhiteSpace(request.PlayerId))
            throw new RequestException(ErrorStatusCode.BadRequest, "INVALID_PLAYER_ID");

        if (string.IsNullOrWhiteSpace(request.Password))
            throw new RequestException(ErrorStatusCode.BadRequest, "INVALID_PASSWORD");

        var account = await _accountRepository.FindByPlayerIdAsync(request.PlayerId);

        if (account == null || string.IsNullOrEmpty(account.Password))
        {
            using var sha = SHA256.Create();
            
            sha.ComputeHash(Encoding.UTF8.GetBytes(s_dummyPassword));

            throw new RequestException(
                ErrorStatusCode.Unauthorized,
                "INVALID_CREDENTIALS");
        }

        if (!BCrypt.Net.BCrypt.Verify(request.Password, account.Password))
        {
            throw new RequestException(
                ErrorStatusCode.Unauthorized,
                "INVALID_CREDENTIALS");
        }

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
}