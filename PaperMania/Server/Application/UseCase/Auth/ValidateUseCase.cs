using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port;
using Server.Application.UseCase.Auth.Command;
using Server.Application.UseCase.Auth.Result;

namespace Server.Application.UseCase.Auth;

public class ValidateUseCase : IValidateUseCase
{
    private readonly ISessionService _sessionService;

    public ValidateUseCase(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }
    
    public async Task<ValidateResult> ExecuteAsync(ValidateCommand request)
    {
        ValidateInput(request);
        var isValidated = await _sessionService.ValidateSessionAsync(request.SessionId);

        return new ValidateResult(
            IsValidated: isValidated
        );
    }
    
    private static void ValidateInput(ValidateCommand request)
    {
        if (string.IsNullOrWhiteSpace(request.SessionId))
        {
            throw new RequestException(
                ErrorStatusCode.BadRequest,
                "INVALID_SESSION_ID");
        }
    }
}