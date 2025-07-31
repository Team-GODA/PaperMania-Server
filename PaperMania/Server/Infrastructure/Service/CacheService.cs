using Server.Application.Port;
using StackExchange.Redis;

namespace Server.Infrastructure.Service;

public class CacheService : ICacheService
{
    private readonly IDatabase _db;

    public CacheService(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }
    
    public async Task SetAsync(string key, string value, TimeSpan? expiration = null)
    {
        await _db.StringSetAsync(key, value, expiration);
    }

    public async Task<string?> GetAsync(string key)
    {
        var value = await _db.StringGetAsync(key);
        return value.IsNullOrEmpty ? null : value.ToString();
    }

    public async Task RemoveAsync(string key)
    {
        await _db.KeyDeleteAsync(key);
    }

    public async Task<bool> ExistsAsync(string key)
    {
        return await _db.KeyExistsAsync(key);
    }

    public async Task SetExpirationAsync(string key, TimeSpan expiration)
    {
        await _db.KeyExpireAsync(key, expiration);
    }
}