using Server.Domain.Entity;
using Server.Infrastructure.Persistence.Model;

namespace Server.Application.Port.Output.Persistence;

public interface IAccountRepository
{
    Task<PlayerAccountData?> FindByUserIdAsync(int userId, CancellationToken ct);
    Task<PlayerAccountData?> FindByPlayerIdAsync(string playerId, CancellationToken ct);
    Task<PlayerAccountData?> FindByEmailAsync(string email, CancellationToken ct);
    Task<bool> ExistsByPlayerIdAsync(string playerId, CancellationToken ct);
    Task<PlayerAccountData> CreateAsync(PlayerAccountData account, CancellationToken ct);
    Task UpdateAsync(PlayerAccountData account, CancellationToken ct);
}