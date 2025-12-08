using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port;
using Server.Application.UseCase.Auth.Command;
using Server.Application.UseCase.Auth.Result;

namespace Server.Application.UseCase.Auth;

public class LoginService : ILoginUseCase
{
    private readonly IAccountRepository _repository;
    private readonly ISessionService _sessionService;
    
    public LoginService(IAccountRepository repository,
        ISessionService sessionService)
    {
        _repository = repository;
        _sessionService = sessionService;
    }
    
    public async Task<LoginResult> ExecuteAsync(LoginCommand request)
    {
        var user = await _repository.GetAccountDataByPlayerIdAsync(request.PlayerId);

        if (user == null)
            throw new RequestException(ErrorStatusCode.NotFound, 
                "USER_NOT_FOUND", new { PlayerId = request.PlayerId });
        
        bool isVerified = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);
        if (!isVerified)
            throw new RequestException(ErrorStatusCode.Conflict, 
                "PASSWORD_NOT_VERIFIED", new { PlayerId = request.PlayerId });

        var sessionId = await _sessionService.CreateSessionAsync(user.Id);

        return new LoginResult(
            SessionId: sessionId,
            IsNewAccount: user.IsNewAccount
        );
    }
}