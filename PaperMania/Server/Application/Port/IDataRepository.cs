using Server.Domain.Entity;

namespace Server.Application.Port;

public interface IDataRepository
{
    Task<PlayerGameData?> ExistsPlayerNameAsync(string playerName);
    Task AddPlayerDataAsync(int? userId, string playerName);
    Task<PlayerGameData?> GetPlayerDataByIdAsync(int? userId);
    Task<PlayerGameData?> UpdatePlayerLevelAsync(int? userId, int newLevel, int newExp);
    Task<LevelDefinition?> GetLevelDataAsync(int currentLevel);
    Task RenamePlayerNameAsync(int? userId, string newPlayerName);
}