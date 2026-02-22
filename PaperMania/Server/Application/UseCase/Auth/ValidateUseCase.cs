using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.Input.Auth;
using Server.Application.Port.Output.Service;

namespace Server.Application.UseCase.Auth;

public class ValidateUseCase : IValidateUseCase
{
    private readonly ISessionService _sessionService;

    public ValidateUseCase(
        ISessionService sessionService
    )
    {
        _sessionService = sessionService;
    }
    
    public async Task ExecuteAsync(string sessionId, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(sessionId) ||
            !await _sessionService.ValidateSessionAsync(sessionId, ct))
            throw new RequestException(
                ErrorStatusCode.Unauthorized,
                "INVALID_SESSION_ID");
    }
}