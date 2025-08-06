using Dapper;
using Server.Application.Port;
using Server.Domain.Entity;

namespace Server.Infrastructure.Repository;

public class AccountRepository : RepositoryBase, IAccountRepository
{
    public AccountRepository(string connectionString) :  base(connectionString)
    {
    }
    
    public async Task<PlayerAccountData?> GetAccountDataByPlayerIdAsync(string playerId)
    {
        var db = CreateConnection();
        
        var sql = @"
            SELECT id AS Id, player_id AS PlayerId, email, password, is_new_account AS IsNewAccount,
                   role AS Role, created_at AS CreatedAt
            FROM paper_mania_account_data.player_account_data
            WHERE player_id = @PlayerId
            LIMIT 1";

        return await db.QueryFirstOrDefaultAsync<PlayerAccountData>(sql, new { PlayerId = playerId });
    }

    public async Task<PlayerAccountData?> GetAccountDataByEmailAsync(string email)
    {
        await using var db = CreateConnection();
        await db.OpenAsync();
        
        var sql = @"
            SELECT id, player_id AS PlayerId, email, password, is_new_account AS IsNewAccount,
                   role AS Role, created_at AS CreatedAt
            FROM paper_mania_account_data.player_account_data
            WHERE email = @Email
            LIMIT 1";
        
        return await db.QueryFirstOrDefaultAsync<PlayerAccountData>(sql, new { Email = email });
    }

    public async Task<PlayerAccountData?> AddAccountAsync(PlayerAccountData player)
    {
        await using var db = CreateConnection();
        await db.OpenAsync();
        
        var sql = @"
            INSERT INTO paper_mania_account_data.player_account_data (player_id, email, password, is_new_account, role)
            VALUES (@PlayerId, @Email, @Password, @IsNewAccount, @Role)
            RETURNING id";
    
        var id = await db.QuerySingleAsync<int>(sql, player);
        player.Id = id;
        return player;
    }
    
    public async Task<bool> IsNewAccountAsync(int? userId)
    {
        await using var db = CreateConnection();
        await db.OpenAsync();
        
        var sql = @"
            SELECT is_new_account AS IsNewAccount
            FROM paper_mania_account_data.player_account_data
            WHERE Id = @Id
            LIMIT 1";
        
        return await db.ExecuteScalarAsync<bool>(sql, new { Id = userId });
    }
    
    public async Task UpdateIsNewAccountAsync(int? userId, bool isNew = true)
    {
        await using var db = CreateConnection();
        await db.OpenAsync();
        
        var sql = @"
            UPDATE paper_mania_account_data.player_account_data
            SET is_new_account = @IsNew
            WHERE id = @Id";

        await db.ExecuteAsync(sql, new { IsNew = isNew, Id = userId });
    }
}