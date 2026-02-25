using Server.Domain.Entity;
using Server.Infrastructure.StaticData.Model;

namespace Server.Application.Port.Output.Persistence;

public interface IDataRepository
{
    Task<PlayerData?> ExistsPlayerNameAsync(string playerName, CancellationToken ct);
    Task CreateAsync(PlayerData player, CancellationToken ct);
    Task<PlayerData?> FindByUserIdAsync(int? userId, CancellationToken ct);
    Task UpdateAsync(PlayerData data, CancellationToken ct);
    Task<LevelDefinition?> FindLevelDataAsync(int currentLevel, CancellationToken ct);
    Task RenamePlayerNameAsync(int? userId, string newPlayerName, CancellationToken ct);
}