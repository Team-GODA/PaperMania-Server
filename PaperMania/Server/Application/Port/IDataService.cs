using Server.Domain.Entity;

namespace Server.Application.Port;

public interface IDataService
{
    Task<string> AddPlayerDataAsync(string playerName, string sessionId);
    Task<string?> GetPlayerNameByUserIdAsync(int? userId);
    Task<int> GetPlayerLevelByUserIdAsync(int? userId);
    Task<int> GetPlayerExpByUserIdAsync(int? userId);
    Task<PlayerGameData> UpdatePlayerLevelByExpAsync(int? userId, int exp);
    Task RenamePlayerNameAsync(int? userId, string? newPlayerName);
}