using Server.Domain.Entity;

namespace Server.Application.Port;

public interface ICurrencyRepository
{
    Task AddPlayerGoodsDataByUserIdAsync(int? userId);
    Task<PlayerCurrencyData> GetPlayerGoodsDataByUserIdAsync(int? userId);
    Task UpdatePlayerGoodsDataAsync(PlayerCurrencyData data);
}