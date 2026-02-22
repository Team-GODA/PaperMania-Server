using Server.Domain.Entity;
using Server.Infrastructure.Persistence.Model;

namespace Server.Application.Port.Output.Persistence;

public interface ICurrencyRepository
{
    Task CreateByUserIdAsync(int userId, CancellationToken ct);
    Task<PlayerCurrencyData?> FindByUserIdAsync(int userId, CancellationToken ct);
    Task UpdateAsync(PlayerCurrencyData data, CancellationToken ct);
    
    Task RegenerateActionPointAsync(int userId, int newActionPoint, DateTime lastUpdated, CancellationToken ct);
    Task SetActionPointToMaxAsync(int userId, CancellationToken ct);
}