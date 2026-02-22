namespace Server.Application.Port.Output.Service;

public interface ICacheService
{
    Task SetAsync(string key, string value, TimeSpan? expiration, CancellationToken ct);
    Task<string?> GetAsync(string key, CancellationToken ct);
    Task DeleteAsync(string key, CancellationToken ct);
    Task<bool> ExistsAsync(string key, CancellationToken ct);
    Task SetExpirationAsync(string key, TimeSpan expiration, CancellationToken ct);

    Task SetHashAsync(string key, string field, string value, CancellationToken ct);
    Task<string?> HashGetAsync(string key, string field, CancellationToken ct);
    Task HashDeleteAsync(string key, string field, CancellationToken ct);
    Task<bool> HashExistsAsync(string key, string field, CancellationToken ct);
}