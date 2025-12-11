namespace Server.Application.Port;

public interface ISessionService
{
    Task<string> CreateSessionAsync(int userId);
    Task<bool> ValidateSessionAsync(string sessionId);
    Task RefreshSessionAsync(string sessionId);
    Task<int?> FindUserIdBySessionIdAsync(string sessionId);
    Task DeleteSessionAsync(string sessionId);
}