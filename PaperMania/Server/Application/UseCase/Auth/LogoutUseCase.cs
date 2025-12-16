using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port;
using Server.Application.UseCase.Auth.Command;

namespace Server.Application.UseCase.Auth;

public class LogoutUseCase : ILogoutUseCase
{
    private readonly ISessionService _sessionService;

    public LogoutUseCase(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }
    
    public async Task ExecuteAsync(LogoutCommand request)
    {
        ValidateInput(request);
        
        var userId = await _sessionService.FindUserIdBySessionIdAsync(request.SessionId);
        if (userId == null)
            return;
        
        await _sessionService.DeleteSessionAsync(request.SessionId);
    }
    
    private static void ValidateInput(LogoutCommand request)
    {
        if (string.IsNullOrWhiteSpace(request.SessionId))
        {
            throw new RequestException(
                ErrorStatusCode.BadRequest,
                "INVALID_SESSION_ID");
        }
    }
}