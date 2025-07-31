using Server.Domain.Entity;

namespace Server.Application.Port;

public interface ICurrencyService
{
    Task<int> GetPlayerActionPointAsync(int? userId);
    Task<int> UpdatePlayerMaxActionPoint(int? userId, int newMaxActionPoint);
    Task UsePlayerActionPointAsync(int? userId, int usedActionPoint);
    
    Task<int> GetPlayerGoldAsync(int? userId);
    Task ModifyPlayerGoldAsync(int? userId, int amount);
    
    Task<int> GetPlayerPaperPieceAsync(int? userId);
    Task ModifyPlayerPaperPieceAsync(int? userId, int amount);
}