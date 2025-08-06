using Server.Domain.Entity;

namespace Server.Application.Port;

public interface ICharacterRepository
{
    Task<IEnumerable<PlayerCharacterData>> GetPlayerCharacterDataByUserIdAsync(int? userId);
    Task<PlayerCharacterData> AddPlayerCharacterDataByUserIdAsync(PlayerCharacterData data);
    Task<bool> IsNewCharacterExistAsync(int userId, string characterId);
    
}