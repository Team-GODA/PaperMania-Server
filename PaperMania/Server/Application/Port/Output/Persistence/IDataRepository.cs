using Server.Domain.Entity;
using Server.Infrastructure.Persistence.Model;
using Server.Infrastructure.StaticData;
using Server.Infrastructure.StaticData.Model;

namespace Server.Application.Port.Output.Persistence;

public interface IDataRepository
{
    Task<PlayerGameData?> ExistsPlayerNameAsync(string playerName, CancellationToken ct);
    Task CreateAsync(PlayerGameData player, CancellationToken ct);
    Task<PlayerGameData?> FindByUserIdAsync(int? userId, CancellationToken ct);
    Task<PlayerGameData?> UpdatePlayerLevelAsync(int? userId, int newLevel, int newExp, CancellationToken ct);
    Task<LevelDefinition?> FindLevelDataAsync(int currentLevel, CancellationToken ct);
    Task RenamePlayerNameAsync(int? userId, string newPlayerName, CancellationToken ct);
}