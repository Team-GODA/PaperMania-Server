using Server.Domain.Entity;

namespace Server.Application.Port;

public interface IDataRepository
{
    Task<PlayerGameData?> ExistsPlayerNameAsync(string playerName);
    Task CreateDataAsync(int? userId, string playerName);
    Task<PlayerGameData?> FindByUserIdAsync(int? userId);
    Task<PlayerGameData?> UpdatePlayerLevelAsync(int? userId, int newLevel, int newExp);
    Task<LevelDefinition?> FindLevelDataAsync(int currentLevel);
    Task RenamePlayerNameAsync(int? userId, string newPlayerName);
}