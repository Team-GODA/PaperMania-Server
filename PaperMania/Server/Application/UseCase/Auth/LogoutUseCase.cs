using Server.Application.Port.Input.Auth;
using Server.Application.Port.Output.Service;

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
    
    public async Task ExecuteAsync(string sessionId, CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(sessionId))
            await _sessionService.DeleteSessionAsync(sessionId, ct);
    }
}