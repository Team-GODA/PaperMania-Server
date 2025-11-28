namespace Server.Application.Port;

public interface ICacheService
{
    Task SetAsync(string key, string value, TimeSpan? expiration = null);
    Task<string?> GetAsync(string key);
    Task RemoveAsync(string key);
    Task<bool> ExistsAsync(string key);
    Task SetExpirationAsync(string key, TimeSpan expiration);

    Task SetHashAsync(string key, string field, string value);
    Task<string?> HashGetAsync(string key, string field);
    Task HashDeleteAsync(string key, string field);
    Task<bool> HashExistsAsync(string key, string field);
}