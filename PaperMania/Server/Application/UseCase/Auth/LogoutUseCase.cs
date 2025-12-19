using Server.Application.Port.In.Auth;
using Server.Application.Port.Out.Service;

namespace Server.Application.UseCase.Auth;

public class LogoutUseCase : ILogoutUseCase
{
    private readonly ISessionService _sessionService;

    public LogoutUseCase(
        ISessionService sessionService
    )
    {
        _sessionService = sessionService;
    }
    
    public async Task ExecuteAsync(string sessionId)
    {
        if (!string.IsNullOrWhiteSpace(sessionId))
            await _sessionService.DeleteSessionAsync(sessionId);
    }
}