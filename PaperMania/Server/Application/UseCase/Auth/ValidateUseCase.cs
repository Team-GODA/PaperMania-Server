using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port;

namespace Server.Application.UseCase.Auth;

public class ValidateUseCase
{
    private readonly ISessionService _sessionService;

    public ValidateUseCase(
        ISessionService sessionService
    )
    {
        _sessionService = sessionService;
    }
    
    public async Task ExecuteAsync(string sessionId)
    {
        if (string.IsNullOrWhiteSpace(sessionId) ||
            !await _sessionService.ValidateSessionAsync(sessionId))
            throw new RequestException(
                ErrorStatusCode.Unauthorized,
                "INVALID_SESSION_ID");
    }
}