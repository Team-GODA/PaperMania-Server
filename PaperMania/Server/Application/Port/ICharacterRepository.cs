using Server.Domain.Entity;

namespace Server.Application.Port;

public interface ICharacterRepository
{
    Task<IEnumerable<PlayerCharacterData>> GetPlayerCharactersDataByUserIdAsync(int userId);
    Task AddPlayerCharacterDataByUserIdAsync(PlayerCharacterData data);
    Task<bool> HasCharacterAsync(int userId, string characterId);
    Task AddCharacterPiecesAsync(int userId, string characterId, int amount);
}