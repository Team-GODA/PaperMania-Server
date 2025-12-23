using Server.Application.Port;
using Server.Application.Port.Output.Service;
using StackExchange.Redis;

namespace Server.Infrastructure.Cache;

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

    public async Task SetAsync(string key, string value, TimeSpan? expiration = null)
    {
        _logger.LogInformation($"Cache SET: {key} = {value}");
        
        await _db.StringSetAsync(key, value, expiration);
    }

    public async Task<string?> GetAsync(string key)
    {
        var value = await _db.StringGetAsync(key);
        
        _logger.LogInformation($"Cache GET: {key} = {value}");
        
        return value.IsNullOrEmpty ? null : value.ToString();
    }

    public async Task DeleteAsync(string key)
    {
        _logger.LogInformation($"Cache DELETE: {key}");
        
        await _db.KeyDeleteAsync(key);
    }

    public async Task<bool> ExistsAsync(string key)
    {
        var exists = await _db.KeyExistsAsync(key);

        _logger.LogInformation($"Cache EXISTS: {key} = {exists}");
        
        return exists;
    }

    public async Task SetExpirationAsync(string key, TimeSpan expiration)
    {
        _logger.LogInformation($"Cache EXPIRE: {key} = {expiration}");
        
        await _db.KeyExpireAsync(key, expiration);
    }

    public async Task SetHashAsync(string key, string field, string value)
    {
        _logger.LogInformation($"Cache SET HSET: {key} -> {field} = {value}");
        await _db.HashSetAsync(key, field, value);
    }

    public async Task<string?> HashGetAsync(string key, string field)
    {
        var value = await _db.HashGetAsync(key, field);
        
        _logger.LogInformation($"Cache GET HSET: {key} -> {field} = {value}");
        return value.IsNullOrEmpty ? null : value.ToString();
    }

    public async Task HashDeleteAsync(string key, string field)
    {
        _logger.LogInformation($"Cache DELETE HSET: {key} -> {field}");
        await _db.HashDeleteAsync(key, field);
    }

    public async Task<bool> HashExistsAsync(string key, string field)
    {
        var exists = await _db.HashExistsAsync(key, field);
        
        _logger.LogInformation($"Cache EXIST HSET: {key} -> {field} = {exists}");
        return exists;
    }
}