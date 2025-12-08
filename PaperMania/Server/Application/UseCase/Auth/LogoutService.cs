using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port;
using Server.Application.UseCase.Auth.Command;

namespace Server.Application.UseCase.Auth;

public class LogoutService : ILogoutUseCase
{
    private readonly ISessionService _sessionService;

    public LogoutService(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }
    
    public async Task ExecuteAsync(LogoutCommand request)
    {
        if (string.IsNullOrWhiteSpace(request.SessionId))
            throw new RequestException(ErrorStatusCode.BadRequest,
                "INVALID_SESSION_ID");
        
        await _sessionService.DeleteSessionAsync(request.SessionId);
    }
}