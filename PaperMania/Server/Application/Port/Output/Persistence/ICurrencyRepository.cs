using Server.Domain.Entity;
using Server.Infrastructure.Persistence.Model;

namespace Server.Application.Port.Output.Persistence;

public interface ICurrencyRepository
{
    Task CreateByUserIdAsync(int userId);
    Task<PlayerCurrencyData?> FindByUserIdAsync(int userId);
    Task UpdateAsync(PlayerCurrencyData data);
    
    Task RegenerateActionPointAsync(int userId, int newActionPoint, DateTime lastUpdated);
    Task SetActionPointToMaxAsync(int userId);
}