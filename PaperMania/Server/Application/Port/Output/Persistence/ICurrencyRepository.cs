namespace Server.Application.Port.Output.Persistence;

public interface ICurrencyRepository
{
    Task CreateByUserIdAsync(int userId, CancellationToken ct);
    Task<Server.Domain.Entity.Currency?> FindByUserIdAsync(int userId, CancellationToken ct);
    Task UpdateAsync(Server.Domain.Entity.Currency currency, CancellationToken ct);
    
    Task RegenerateActionPointAsync(int userId, int newActionPoint, DateTime lastUpdated, CancellationToken ct);
    Task SetActionPointToMaxAsync(int userId, CancellationToken ct);
}