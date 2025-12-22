using Server.Domain.Entity;
using Server.Infrastructure.Persistence.Model;

namespace Server.Application.Port.Output.Persistence;

public interface ICharacterDao
{
    Task<IEnumerable<PlayerCharacterData>> GetPlayerCharactersDataByUserIdAsync(int userId);
    Task AddPlayerCharacterDataByUserIdAsync(PlayerCharacterData data);
    Task<bool> HasCharacterAsync(int userId, string characterId);
    Task AddCharacterPiecesAsync(int userId, string characterId, int amount);
}