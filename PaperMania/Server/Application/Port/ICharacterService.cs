using Server.Domain.Entity;

namespace Server.Application.Port;

public interface ICharacterService
{
    Task<IEnumerable<PlayerCharacterData>> GetPlayerCharacterDataByUserIdAsync(int userId);
    Task<PlayerCharacterData> AddPlayerCharacterDataByUserIdAsync(PlayerCharacterData data);
}