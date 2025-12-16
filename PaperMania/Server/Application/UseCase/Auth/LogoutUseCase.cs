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
        await _sessionService.DeleteSessionAsync(request.SessionId);
    }
}