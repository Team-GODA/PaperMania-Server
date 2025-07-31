using Server.Domain.Entity;

namespace Server.Application.Port;

public interface ICurrencyService
{
    Task<int> GetPlayerActionPointAsync(int? userId);
    Task<int> UpdatePlayerMaxActionPoint(int? userId, int newMaxActionPoint);
    Task UsePlayerActionPointAsync(int? userId, int usedActionPoint);
    
    Task<int> GetPlayerGoldAsync(int? userId);
    Task AddPlayerGoldAsync(int? userId, int gold);
    Task UsePlayerGoldAsync(int? userId, int usedGold);
    
    Task<int> GetPlayerPaperPieceAsync(int? userId);
    Task AddPlayerPaperPieceAsync(int? userId, int paperPiece);
    Task UsePlayerPaperPieceAsync(int? userId, int usedPaperPiece);
}