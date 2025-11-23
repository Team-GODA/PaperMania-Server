namespace Server.Application.Port;

public interface ISessionService
{
    Task<string> CreateSessionAsync(int userId);
    Task<bool> ValidateSessionAsync(string sessionId, int? userId = null);
    Task<bool> ValidateAndRefreshSessionAsync(string sessionId, int? userId = null);
    Task<int> GetUserIdBySessionIdAsync(string sessionId);
    Task DeleteSessionAsync(string sessionId);
}