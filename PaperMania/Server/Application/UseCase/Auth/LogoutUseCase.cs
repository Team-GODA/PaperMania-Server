using Server.Application.Port;

namespace Server.Application.UseCase.Auth;

public class LogoutUseCase
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