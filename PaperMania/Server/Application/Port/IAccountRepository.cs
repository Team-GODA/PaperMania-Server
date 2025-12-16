using Server.Domain.Entity;

namespace Server.Application.Port;

public interface IAccountRepository
{
    Task<PlayerAccountData?> FindByPlayerIdAsync(string playerId);
    Task<PlayerAccountData?> FindByEmailAsync(string email);
    Task<PlayerAccountData> CreateAccountAsync(PlayerAccountData? account);
    Task<bool> IsNewAccountAsync(int? userId);
    Task UpdateIsNewAccountAsync(int? userId, bool isNew = true);
}