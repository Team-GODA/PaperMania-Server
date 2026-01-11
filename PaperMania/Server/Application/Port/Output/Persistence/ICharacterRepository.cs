using Server.Infrastructure.Persistence.Model;

namespace Server.Application.Port.Output.Persistence;

public interface ICharacterRepository
{
    Task<IEnumerable<PlayerCharacterData>> FindAll(int userId);
    Task<PlayerCharacterData?> FindCharacter(int userId, int characterId);
    Task<PlayerCharacterData> UpdateAsync(PlayerCharacterData data);
    Task CreateAsync(PlayerCharacterData data);
    Task CreatePieceData(PlayerCharacterPieceData data);
}