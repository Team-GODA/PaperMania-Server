using Server.Domain.Entity;

namespace Server.Application.Port;

public interface ICurrencyRepository
{
    Task AddPlayerCurrencyDataByUserIdAsync(int userId);
    Task<PlayerCurrencyData> FindPlayerCurrencyDataByUserIdAsync(int userId);
    Task UpdatePlayerCurrencyDataAsync(PlayerCurrencyData data);
}