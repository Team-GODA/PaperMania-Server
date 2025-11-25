namespace Server.Application.Port;

public interface ICacheService
{
    Task SetAsync(string key, string value, TimeSpan? expiration = null, string? prefix = null);
    Task<string?> GetAsync(string key, string? prefix = null);
    Task RemoveAsync(string key, string? prefix = null);
    Task<bool> ExistsAsync(string key, string? prefix = null);
    Task SetExpirationAsync(string key, TimeSpan expiration, string? prefix = null);
}