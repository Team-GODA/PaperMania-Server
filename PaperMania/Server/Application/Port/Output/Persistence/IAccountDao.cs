using Server.Domain.Entity;
using Server.Infrastructure.Persistence.Model;

namespace Server.Application.Port.Output.Persistence;

public interface IAccountDao
{
    Task<PlayerAccountData?> FindByUserIdAsync(int userId);
    Task<PlayerAccountData?> FindByPlayerIdAsync(string playerId);
    Task<PlayerAccountData?> FindByEmailAsync(string email);
    Task<bool> ExistsByPlayerIdAsync(string playerId);
    Task<PlayerAccountData> CreateAsync(PlayerAccountData account);
    Task UpdateAsync(PlayerAccountData account);
}