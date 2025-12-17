using Dapper;
using Server.Application.Port;
using Server.Application.Port.Out.Infrastructure;
using Server.Application.Port.Out.Persistence;
using Server.Domain.Entity;

namespace Server.Infrastructure.Repository;

public class CurrencyRepository : RepositoryBase, ICurrencyRepository
{
    private static class Sql
    {
        public const string AddPlayerCurrencyData = @"
            INSERT INTO paper_mania_game_data.player_currency_data (user_id)
            VALUES (@UserId)
            ";
        
        public const string GetPlayerCurrencyData = @"
            SELECT user_id AS UserId, action_point AS ActionPoint, action_point_max AS MaxActionPoint, 
                gold AS Gold, paper_piece AS PaperPiece, last_action_point_updated AS LastActionPointUpdated
            FROM paper_mania_game_data.player_currency_data
            WHERE user_id = @UserId
            ";
        
        public const string UpdatePlayerCurrencyData = @"
            UPDATE paper_mania_game_data.player_currency_data
            SET action_point = @ActionPoint,
                action_point_max = @MaxActionPoint,
                last_action_point_updated = @LastActionPointUpdated,
                gold = @Gold,
                paper_piece = @PaperPiece
            WHERE user_id = @UserId
            ";

        public const string RegenerateActionPoint = @"
            UPDATE paper_mania_game_data.player_currency_data
            SET action_point = @NewActionPoint,
                last_action_point_updated = @LastUpdated
            WHERE user_id = @UserId
            ";

        public const string SetActionPointToMax = @"
            UPDATE paper_mania_game_data.player_currency_data
            SET action_point = action_point_max,
                last_action_point_updated = @LastUpdated
            WHERE user_id = @UserId
            ";
    }
    
    public CurrencyRepository(
        string connectionString,
        ITransactionScope? transactionScope = null) 
        : base(connectionString, transactionScope)
    {
    }
    
    public async Task CreateByUserIdAsync(int userId)
    {
        await ExecuteAsync((connection, transaction) =>
             connection.ExecuteAsync(
                Sql.AddPlayerCurrencyData,
                new { UserId = userId },
                transaction)
             );
    }

    public async Task<PlayerCurrencyData?> FindByUserIdAsync(int userId)
    {
        return await QueryAsync(connection =>
            connection.QueryFirstOrDefaultAsync<PlayerCurrencyData>(
                Sql.GetPlayerCurrencyData,
                new { UserId = userId }
                )
            );
    }

    public async Task UpdateDataAsync(PlayerCurrencyData data)
    {
        await ExecuteAsync((connection, transaction) =>
            connection.ExecuteAsync(
                Sql.UpdatePlayerCurrencyData,
                data,
                transaction)
            );
    }

    public async Task RegenerateActionPointAsync(int userId, 
        int newActionPoint,
        DateTime lastUpdated
        )
    {
        await ExecuteAsync((connection, transaction) =>
            connection.ExecuteAsync(
                Sql.RegenerateActionPoint,
                new 
                {
                    UserId = userId, 
                    NewActionPoint = newActionPoint,
                    LastUpdated = lastUpdated.ToUniversalTime()
                },
                transaction)
        );
    }

    public async Task SetActionPointToMaxAsync(int userId)
    {
        await ExecuteAsync((connection, transaction) =>
            connection.ExecuteAsync(
                Sql.SetActionPointToMax,
                new
                {
                    UserId = userId,
                    LastUpdated = DateTime.UtcNow
                },
                transaction)
        );
    }
}