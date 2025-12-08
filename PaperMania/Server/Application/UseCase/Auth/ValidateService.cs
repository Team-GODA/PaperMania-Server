using Server.Application.Port;
using Server.Application.UseCase.Auth.Command;
using Server.Application.UseCase.Auth.Result;

namespace Server.Application.UseCase.Auth;

public class ValidateService : IValidateUseCase
{
    private readonly ISessionService _sessionService;

    public ValidateService(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }
    
    public async Task<ValidateResult> ExecuteAsync(ValidateCommand request)
    {
        var userId = await _sessionService.GetUserIdBySessionIdAsync(request.SessionId);
        
        return new ValidateResult(
            UserId: userId,
            IsValidated: true
            );
    }
}