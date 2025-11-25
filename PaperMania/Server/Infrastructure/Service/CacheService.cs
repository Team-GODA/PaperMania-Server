using Server.Application.Port;
using StackExchange.Redis;

namespace Server.Infrastructure.Service;

public class CacheService : ICacheService
{
    private readonly IDatabase _db;
    private readonly ILogger<CacheService> _logger;

    public CacheService(IConnectionMultiplexer redis,
        ILogger<CacheService> logger)
    {
        _db = redis.GetDatabase();
        _logger = logger;
    }

    private string BuildKey(string key, string? prefix)
    {
        return string.IsNullOrEmpty(prefix) ? key : $"{prefix}:{key}";
    }
    
    public async Task SetAsync(string key, string value, TimeSpan? expiration = null, string? prefix = null)
    {
        var cachekey = BuildKey(key, prefix);
        _logger.LogInformation($"Cache SET: {cachekey} = {value}");
        
        await _db.StringSetAsync(cachekey, value, expiration);
    }

    public async Task<string?> GetAsync(string key, string? prefix = null)
    {
        var cachekey = BuildKey(key, prefix);
        var value = await _db.StringGetAsync(key);
        
        _logger.LogInformation($"Cache GET: {cachekey} = {value}");
        
        return value.IsNullOrEmpty ? null : value.ToString();
    }

    public async Task RemoveAsync(string key, string? prefix = null)
    {
        var cachekey = BuildKey(key, prefix);
        _logger.LogInformation($"Cache DELETE: {cachekey}");
        
        await _db.KeyDeleteAsync(cachekey);
    }

    public async Task<bool> ExistsAsync(string key, string? prefix = null)
    {
        var cachekey = BuildKey(key, prefix);
        var exists = await _db.KeyExistsAsync(cachekey);

        _logger.LogInformation($"Cache EXISTS: {cachekey} = {exists}");
        
        return exists;
    }

    public async Task SetExpirationAsync(string key, TimeSpan expiration, string? prefix = null)
    {
        var cachekey = BuildKey(key, prefix);
        _logger.LogInformation($"Cache EXPIRE: {cachekey} = {expiration}");
        
        await _db.KeyExpireAsync(key, expiration);
    }
}