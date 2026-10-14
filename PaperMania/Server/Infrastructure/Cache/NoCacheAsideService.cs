using Server.Application.Port.Output.Cache;

namespace Server.Infrastructure.Cache;

/// <summary>
/// 캐시를 거치지 않고 항상 원본 조회 (Cache:Enabled=false, 캐싱 전후 성능 비교용)
/// </summary>
public class NoCacheAsideService : ICacheAsideService
{
    public async Task<T?> GetOrSetAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> valueFactory,
        TimeSpan? expiration,
        CancellationToken cancellationToken)
        => await valueFactory(cancellationToken);
}
