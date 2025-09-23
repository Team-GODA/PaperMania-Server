namespace Server.Application.Port;

public interface ICurrencyService
{
    Task<int> GetPlayerActionPointAsync(int? userId);
    Task<int> UpdatePlayerMaxActionPoint(int? userId, int newMaxActionPoint);
    Task UsePlayerActionPointAsync(int? userId, int usedActionPoint);
    
    Task<int> GetPlayerGoldAsync(int? userId);
    Task<int> GetPlayerPaperPieceAsync(int? userId);
    
    Task ModifyPlayerGoldAsync(int? userId, int amount);
    Task ModifyPlayerPaperPieceAsync(int? userId, int amount);
}