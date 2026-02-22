using Server.Infrastructure.Persistence.Model;

namespace Server.Application.Port.Output.Persistence;

public interface ICharacterRepository
{
    Task<IEnumerable<PlayerCharacterData>> FindAll(int userId, CancellationToken ct);
    Task<PlayerCharacterData?> FindCharacter(int userId, int characterId, CancellationToken ct);
    Task<PlayerCharacterData> UpdateAsync(PlayerCharacterData data, CancellationToken ct);
    Task CreateAsync(PlayerCharacterData data, CancellationToken ct);
    Task CreatePieceData(PlayerCharacterPieceData data, CancellationToken ct);
}