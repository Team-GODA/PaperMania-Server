using System.Collections.Concurrent;
using System.Text.Json;
using Server.Application.Port.Output.Cache;
using Server.Application.Port.Output.Service;

namespace Server.Infrastructure.Cache;

public class CacheAsideService : ICacheAsideService
{
    private readonly ICacheService _cacheService;
    private readonly JsonSerializerOptions _jsonOptions;

    private sealed class SemaphoreEntry
    {
        public SemaphoreSlim Semaphore { get; } = new(1, 1);
        public int RefCount;
    }
    
    private static readonly ConcurrentDictionary<string, SemaphoreEntry> _locks = new();

    public CacheAsideService(
        ICacheService cacheService,
        JsonSerializerOptions? jsonOptions = null)
    {
        _cacheService = cacheService;
        _jsonOptions = jsonOptions ?? JsonSerializerOptions.Default;
    }

    public async Task<T?> GetOrSetAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> valueFactory,
        TimeSpan? expiration,
        CancellationToken cancellationToken)
    {
        var cached = await _cacheService.GetAsync(key, cancellationToken);
        if (cached != null)
            return JsonSerializer.Deserialize<T>(cached, _jsonOptions);

        var entry = _locks.AddOrUpdate(
            key,
            _ => { var e = new SemaphoreEntry(); Interlocked.Increment(ref e.RefCount); return e; },
            (_, e) => { Interlocked.Increment(ref e.RefCount); return e; }
        );

        await entry.Semaphore.WaitAsync(cancellationToken);
        try
        {
            cached = await _cacheService.GetAsync(key, cancellationToken);
            if (cached != null)
                return JsonSerializer.Deserialize<T>(cached, _jsonOptions);

            var data = await valueFactory(cancellationToken);

            if (data != null)
            {
                await _cacheService.SetAsync(
                    key,
                    JsonSerializer.Serialize(data, _jsonOptions),
                    expiration,
                    cancellationToken);
            }

            return data;
        }
        finally
        {
            entry.Semaphore.Release();

            if (Interlocked.Decrement(ref entry.RefCount) == 0)
                _locks.TryRemove(key, out _);
        }
    }
}