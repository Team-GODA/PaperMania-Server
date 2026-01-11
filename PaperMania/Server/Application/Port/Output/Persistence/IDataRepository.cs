using Server.Domain.Entity;
using Server.Infrastructure.Persistence.Model;
using Server.Infrastructure.StaticData;
using Server.Infrastructure.StaticData.Model;

namespace Server.Application.Port.Output.Persistence;

public interface IDataRepository
{
    Task<PlayerGameData?> ExistsPlayerNameAsync(string playerName);
    Task CreateAsync(PlayerGameData player);
    Task<PlayerGameData?> FindByUserIdAsync(int? userId);
    Task<PlayerGameData?> UpdatePlayerLevelAsync(int? userId, int newLevel, int newExp);
    Task<LevelDefinition?> FindLevelDataAsync(int currentLevel);
    Task RenamePlayerNameAsync(int? userId, string newPlayerName);
}