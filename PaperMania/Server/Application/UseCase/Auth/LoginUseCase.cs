using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port;
using Server.Application.UseCase.Auth.Command;
using Server.Application.UseCase.Auth.Result;
using Server.Domain.Service;

namespace Server.Application.UseCase.Auth;

public class LoginUseCase
{
    private readonly IAccountRepository _repository;
    private readonly ISessionService _sessionService;
    private readonly UserService _userService;

    public LoginUseCase(
        IAccountRepository repository,
        ISessionService sessionService,
        UserService userService
    )
    {
        _repository = repository;
        _sessionService = sessionService;
        _userService = userService;
    }
    
    public async Task<LoginResult> ExecuteAsync(LoginCommand request)
    {
        request.Validate();
        
        var account = await _repository.FindByPlayerIdAsync(request.PlayerId);
        if (account == null || string.IsNullOrEmpty(account.Password))
        {
            _userService.VerifyPassword(request.Password, "DUMMY_HASH");
            throw new RequestException(
                ErrorStatusCode.Unauthorized,
                "INVALID_CREDENTIALS");
        }

        if (!_userService.VerifyPassword(request.Password, account.Password))
            throw new RequestException(
                ErrorStatusCode.Unauthorized,
                "INVALID_PASSWORD");
        
        var sessionId = await _sessionService.CreateSessionAsync(account.Id);
        if (string.IsNullOrEmpty(sessionId))
            throw new RequestException(
                ErrorStatusCode.ServerError,
                "SESSION_CREATE_FAILED");
        
        return new LoginResult(
            sessionId,
            account.IsNewAccount
        );
    }
}