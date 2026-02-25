using Server.Domain.Entity;
using Server.Infrastructure.Persistence.Model;

namespace Server.Application.Port.Output.Persistence;

public interface IAccountRepository
{
    Task<Account?> FindByUserIdAsync(int userId, CancellationToken ct);
    Task<Account?> FindByPlayerIdAsync(string playerId, CancellationToken ct);
    Task<Account?> FindByEmailAsync(string email, CancellationToken ct);
    Task<bool> ExistsByPlayerIdAsync(string playerId, CancellationToken ct);
    Task<Account?> CreateAsync(Account account, CancellationToken ct);
    Task UpdateAsync(Account account, CancellationToken ct);
}   