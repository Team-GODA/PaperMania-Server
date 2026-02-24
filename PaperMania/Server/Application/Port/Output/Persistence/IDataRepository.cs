using Server.Domain.Entity;
using Server.Infrastructure.StaticData.Model;

namespace Server.Application.Port.Output.Persistence;

public interface IDataRepository
{
    Task<GameData?> ExistsPlayerNameAsync(string playerName, CancellationToken ct);
    Task CreateAsync(GameData player, CancellationToken ct);
    Task<GameData?> FindByUserIdAsync(int? userId, CancellationToken ct);
    Task UpdateAsync(GameData data, CancellationToken ct);
    Task<LevelDefinition?> FindLevelDataAsync(int currentLevel, CancellationToken ct);
    Task RenamePlayerNameAsync(int? userId, string newPlayerName, CancellationToken ct);
}