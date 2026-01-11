using System.Text.Json;
using Server.Application.Port.Output.Service;

namespace Server.Infrastructure.Cache;

public class CacheAsideService
{
    private readonly ICacheService _cacheService;

    public CacheAsideService(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task<T?> GetOrSetAsync<T>(
        string key,
        Func<Task<T?>> valueFactory,
        TimeSpan? expiration = null) where T : class
    {
        var cached = await _cacheService.GetAsync(key);
        if (cached != null)
            return JsonSerializer.Deserialize<T>(cached);
        
        var data = await valueFactory();
        if (data != null)
            await _cacheService.SetAsync(
                key,
                JsonSerializer.Serialize(data),
                expiration);
        
        return data;
    }
}