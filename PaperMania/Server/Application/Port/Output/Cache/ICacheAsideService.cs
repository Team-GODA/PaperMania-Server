namespace Server.Application.Port.Output.Cache;

public interface ICacheAsideService
{
    Task<T?> GetOrSetAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> valueFactory,
        TimeSpan? expiration,
        CancellationToken cancellationToken);
}