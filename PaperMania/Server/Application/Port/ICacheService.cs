namespace Server.Application.Port;

public interface ICacheService
{
    Task SetAsync(string key, string value, TimeSpan? expiration = null, string? prefix = null);
    Task<string?> GetAsync(string key);
    Task RemoveAsync(string key);
    Task<bool> ExistsAsync(string key);
    Task SetExpirationAsync(string key, TimeSpan expiration);
}