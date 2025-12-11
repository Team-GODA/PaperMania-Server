using Server.Api.Dto.Response;
using Server.Application.Exceptions;
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
        var isValid = await _sessionService.ValidateSessionAsync(request.SessionId);
        if (!isValid)
            throw new RequestException(
                ErrorStatusCode.Unauthorized,
                "INVALID_SESSION"
            );
            
        var userId = await _sessionService.FindUserIdBySessionIdAsync(request.SessionId);
        if (userId == null)
            throw new RequestException(
                ErrorStatusCode.ServerError,
                "SESSION_DATA_CORRUPTED");

        return new ValidateResult(
            UserId: userId,
            IsValidated: true
        );
    }
}