using Dapper;
using Server.Application.Port.Output.Persistence;
using Server.Application.Port.Output.Transaction;
using Server.Domain.Entity;
using Server.Infrastructure.Persistence.Model;

namespace Server.Infrastructure.Persistence.Repository;

public class AccountRepository : RepositoryBase, IAccountRepository
{
    private static class Sql
    {
        public const string GetByUserId = @"
            SELECT id AS Id, player_id AS PlayerId, email AS Email, 
                   password AS Password, is_new_account AS IsNewAccount,
                   role AS Role, created_at AS CreatedAt
            FROM paper_mania_account_data.player_account_data
            WHERE id = @UserId
            LIMIT 1";
        
        public const string GetByPlayerId = @"
            SELECT id AS Id, player_id AS PlayerId, email AS Email, 
                   password AS Password, is_new_account AS IsNewAccount,
                   role AS Role, created_at AS CreatedAt
            FROM paper_mania_account_data.player_account_data
            WHERE player_id = @PlayerId
            LIMIT 1";

        public const string GetByEmail = @"
            SELECT id AS Id, player_id AS PlayerId, email AS Email, 
                   password AS Password, is_new_account AS IsNewAccount,
                   role AS Role, created_at AS CreatedAt
            FROM paper_mania_account_data.player_account_data
            WHERE email = @Email
            LIMIT 1";

        public const string InsertAccount = @"
            INSERT INTO paper_mania_account_data.player_account_data 
                (player_id, email, password, is_new_account, role)
            VALUES (@PlayerId, @Email, @Password, @IsNewAccount, @Role)
            RETURNING id AS Id, player_id AS PlayerId, email AS Email, 
                      password AS Password, is_new_account AS IsNewAccount,
                      role AS Role, created_at AS CreatedAt";
        
        public const string ExistsByPlayerId = @"
            SELECT 1
            FROM paper_mania_account_data.player_account_data
            WHERE player_id = @PlayerId
            LIMIT 1";

        public const string UpdateAccount = @"
            UPDATE paper_mania_account_data.player_account_data
            SET
                player_id = @PlayerId,
                email = @Email,
                password = @Password,
                is_new_account = @IsNewAccount,
                role = @Role
            WHERE id = @Id";
    }
    
    public AccountRepository(
        string connectionString, 
        ITransactionScope? transactionScope = null) 
        : base(connectionString, transactionScope)
    {
    }
    
    private static Account? MapToEntity(PlayerAccountData? data)
    {
        if (data == null) return null;
        
        return new Account(
            data.PlayerId,
            data.Email,
            data.Password,
            data.IsNewAccount
        );
    }
    
    public async Task<Account?> FindByUserIdAsync(int userId, CancellationToken ct)
    {
        var data = await QueryAsync(connection =>
            connection.QueryFirstOrDefaultAsync<PlayerAccountData>(
                new CommandDefinition(Sql.GetByUserId, new { UserId = userId }, cancellationToken: ct)
            ), ct);

        return MapToEntity(data);
    }
    
    public async Task<Account?> FindByPlayerIdAsync(string playerId, CancellationToken ct)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(playerId);
        
        var data =  await QueryAsync(connection =>
            connection.QueryFirstOrDefaultAsync<PlayerAccountData>(
                new CommandDefinition(Sql.GetByPlayerId, new { PlayerId = playerId }, cancellationToken: ct)
            ), ct);
        
        return MapToEntity(data);
    }

    public async Task<Account?> FindByEmailAsync(string email, CancellationToken ct)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        
        var data = await QueryAsync(connection =>
            connection.QueryFirstOrDefaultAsync<PlayerAccountData>(
                new CommandDefinition(Sql.GetByEmail, new { Email = email }, cancellationToken: ct)
            ), ct);
        
        return MapToEntity(data);
    }
    
    public async Task<bool> ExistsByPlayerIdAsync(string playerId, CancellationToken ct)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(playerId);

        var result = await QueryAsync(connection =>
            connection.ExecuteScalarAsync<int?>(
                new CommandDefinition(Sql.ExistsByPlayerId, new { PlayerId = playerId }, cancellationToken: ct)
            ), ct);

        return result.HasValue;
    }

    public async Task<Account?> CreateAsync(Account account, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(account);
        
        var data = await ExecuteAsync((connection, transaction) =>
            connection.QuerySingleAsync<PlayerAccountData>(
                new CommandDefinition(Sql.InsertAccount, account, transaction: transaction, cancellationToken: ct)
            ), ct);
        
        return MapToEntity(data);
    }
    
    
    public async Task UpdateAsync(Account account, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(account);

        var rows = await ExecuteAsync((conn, transaction) =>
            conn.ExecuteAsync(
                new CommandDefinition(Sql.UpdateAccount, account, transaction: transaction, cancellationToken: ct)
            ), ct);

        if (rows == 0)
            throw new InvalidOperationException($"ACCOUNT_NOT_FOUND: id={account.Id}");
    }
}