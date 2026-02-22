namespace Server.Application.Port.Output.Service;

public interface ISessionService
{
    Task<string> CreateSessionAsync(int userId, CancellationToken ct);
    Task<bool> ValidateSessionAsync(string sessionId, CancellationToken ct);
    Task RefreshSessionAsync(string sessionId, CancellationToken ct);
    Task<int> FindUserIdBySessionIdAsync(string sessionId, CancellationToken ct);
    Task DeleteSessionAsync(string sessionId, CancellationToken ct);
}