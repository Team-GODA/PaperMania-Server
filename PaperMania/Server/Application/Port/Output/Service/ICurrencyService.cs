namespace Server.Application.Port.Output.Service;

public interface ICurrencyService
{
    Task<int> FindPlayerActionPointAsync(int? userId);
    Task<int> UpdatePlayerMaxActionPoint(int? userId, int newMaxActionPoint);
    Task UsePlayerActionPointAsync(int? userId, int usedActionPoint);
    
    Task<int> FindPlayerGoldAsync(int? userId);
    Task<int> FindPlayerPaperPieceAsync(int? userId);
    
    Task ModifyPlayerGoldAsync(int? userId, int amount);
    Task ModifyPlayerPaperPieceAsync(int? userId, int amount);
}