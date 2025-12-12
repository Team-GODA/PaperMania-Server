using Dapper;
using Server.Application.Port;
using Server.Domain.Entity;

namespace Server.Infrastructure.Repository;

public class AccountRepository : RepositoryBase, IAccountRepository
{
    private static class Sql
    {
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

        public const string IsNewAccount = @"
            SELECT is_new_account
            FROM paper_mania_account_data.player_account_data
            WHERE id = @UserId
            LIMIT 1";

        public const string UpdateIsNewAccount = @"
            UPDATE paper_mania_account_data.player_account_data
            SET is_new_account = @IsNew
            WHERE id = @UserId";
    }
    
    public AccountRepository(
        string connectionString, 
        IUnitOfWork? unitOfWork = null) 
        : base(connectionString, unitOfWork)
    {
    }
    
    public async Task<PlayerAccountData?> FindByPlayerIdAsync(string playerId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(playerId);
        
        return await QueryAsync(connection =>
            connection.QueryFirstOrDefaultAsync<PlayerAccountData>(
                Sql.GetByPlayerId, 
                new { PlayerId = playerId }
            )
        );
    }

    public async Task<PlayerAccountData?> FindByEmailAsync(string email)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        
        return await QueryAsync(connection =>
            connection.QueryFirstOrDefaultAsync<PlayerAccountData>(
                Sql.GetByEmail, 
                new { Email = email }
            )
        );
    }

    public async Task<PlayerAccountData> AddAccountAsync(PlayerAccountData? account)
    {
        ArgumentNullException.ThrowIfNull(account);
        
        return await ExecuteAsync((connection, transaction) =>
            connection.QuerySingleAsync<PlayerAccountData>(
                Sql.InsertAccount, 
                account,
                transaction)
        );
    }
    
    public async Task<bool> IsNewAccountAsync(int userId)
    {
        if (userId <= 0)
            throw new ArgumentException("UserId must be positive", nameof(userId));
        
        var result = await ExecuteAsync((connection, transaction) =>
            connection.ExecuteScalarAsync<bool?>(
                Sql.IsNewAccount, 
                new { UserId = userId },
                transaction)
        );
        
        if (result == null)
            throw new InvalidOperationException($"ACCOUNT_NOT_FOUND: userId = {userId}");
        
        return result.Value;
    }
    
    public async Task UpdateIsNewAccountAsync(int userId, bool isNew)
    {
        if (userId <= 0)
            throw new ArgumentException("UserId must be positive", nameof(userId));
        
        var rowsAffected = await ExecuteAsync((connection, transaction) =>
            connection.ExecuteAsync(
                Sql.UpdateIsNewAccount,
                new { IsNew = isNew, UserId = userId },
                transaction)
        );
        
        if (rowsAffected == 0)
            throw new InvalidOperationException($"ACCOUNT_NOT_FOUND: userId = {userId}");
    }
}