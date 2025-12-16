using Server.Domain.Entity;

namespace Server.Application.Port;

public interface IPlayerRepository
{
    Task<PlayerGameData?> FindByUserIdAsync(long userId);
    Task<PlayerGameData?> FindByNameAsync(string playerName);
    Task CreateAsync(PlayerGameData player);
    Task UpdateAsync(PlayerGameData player);
}