using Server.Domain.Entity;

namespace Server.Application.Port;

public interface ICurrencyRepository
{
    Task AddPlayerCurrencyDataByUserIdAsync(int? userId);
    Task<PlayerCurrencyData> GetPlayerCurrencyDataByUserIdAsync(int? userId);
    Task UpdatePlayerCurrencyDataAsync(PlayerCurrencyData data);
}