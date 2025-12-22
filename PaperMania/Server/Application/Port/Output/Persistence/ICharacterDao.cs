using Server.Domain.Entity;
using Server.Infrastructure.Persistence.Model;

namespace Server.Application.Port.Output.Persistence;

public interface ICharacterDao
{
    Task<IEnumerable<PlayerCharacterData?>> FindAll(int userId);
    Task<PlayerCharacterData?> FindCharacter(int userId, int characterId);
    Task<PlayerCharacterData?> UpdateAsync(PlayerCharacterData data);
    Task<PlayerCharacterData?> CreateAsync(PlayerCharacterData data);
}