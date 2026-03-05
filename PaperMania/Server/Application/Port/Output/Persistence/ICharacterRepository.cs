using Server.Domain.Entity;
using Server.Infrastructure.Persistence.Model;

namespace Server.Application.Port.Output.Persistence;

public interface ICharacterRepository
{
    Task<IEnumerable<PlayerCharacter>> FindAll(int userId, CancellationToken ct);
    Task<PlayerCharacter?> FindCharacter(int userId, int characterId, CancellationToken ct);
    Task<PlayerCharacter> UpdateAsync(PlayerCharacter entity, CancellationToken ct);
    Task CreateAsync(PlayerCharacter entity, CancellationToken ct);
    Task CreatePieceData(PlayerCharacter entity, CancellationToken ct);
}