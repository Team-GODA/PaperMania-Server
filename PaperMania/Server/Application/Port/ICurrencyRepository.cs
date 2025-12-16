using Server.Domain.Entity;

namespace Server.Application.Port;

public interface ICurrencyRepository
{
    Task CreateByUserIdAsync(int userId);
    Task<PlayerCurrencyData?> FindByUserIdAsync(int userId);
    Task UpdateDataAsync(PlayerCurrencyData data);
}