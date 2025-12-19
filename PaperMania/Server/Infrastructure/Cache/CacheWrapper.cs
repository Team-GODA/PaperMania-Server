using System.Text.Json;
using Server.Application.Port;
using Server.Application.Port.Output.Service;

namespace Server.Infrastructure.Cache;

public class CacheWrapper
{
    private readonly ICacheService _cacheService;

    public CacheWrapper(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task<T?> FetchAsync<T>(
        string key,
        Func<Task<T?>> fetchFunc,
        TimeSpan? expiration = null) where T : class
    {
        var cached = await _cacheService.GetAsync(key);
        if (cached != null)
            return JsonSerializer.Deserialize<T>(cached);
        
        var data = await fetchFunc();
        if (data != null)
            await _cacheService.SetAsync(
                key,
                JsonSerializer.Serialize(data),
                expiration);
        
        return data;
    }
}