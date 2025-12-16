using Server.Domain.Entity;

namespace Server.Application.Port;

public interface IAccountRepository
{
    Task<PlayerAccountData?> FindByPlayerIdAsync(string playerId);
    Task<PlayerAccountData?> FindByEmailAsync(string email);
    Task<bool> ExistsByPlayerIdAsync(string playerId);
    Task<PlayerAccountData> CreateAsync(PlayerAccountData account);
    Task UpdateAsync(PlayerAccountData account);
}