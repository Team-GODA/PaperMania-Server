using Dapper;
using Server.Application.Port.Output.Persistence;
using Server.Application.Port.Output.Transaction;
using Server.Infrastructure.Persistence.Model;

namespace Server.Infrastructure.Persistence.Dao;

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
    
    public async Task CreateByUserIdAsync(int userId, CancellationToken ct)
    {
        await ExecuteAsync((connection, transaction) =>
             connection.ExecuteAsync(
                new CommandDefinition(Sql.AddPlayerCurrencyData, new { UserId = userId }, transaction: transaction, cancellationToken: ct)
             ), ct);
    }

    public async Task<PlayerCurrencyData?> FindByUserIdAsync(int userId, CancellationToken ct)
    {
        return await QueryAsync(connection =>
            connection.QueryFirstOrDefaultAsync<PlayerCurrencyData>(
                new CommandDefinition(Sql.GetPlayerCurrencyData, new { UserId = userId }, cancellationToken: ct)
            ), ct);
    }

    public async Task UpdateAsync(PlayerCurrencyData data, CancellationToken ct)
    {
        await ExecuteAsync((connection, transaction) =>
            connection.ExecuteAsync(
                new CommandDefinition(Sql.UpdatePlayerCurrencyData, data, transaction: transaction, cancellationToken: ct)
            ), ct);
    }

    public async Task RegenerateActionPointAsync(int userId, 
        int newActionPoint,
        DateTime lastUpdated,
        CancellationToken ct)
    {
        await ExecuteAsync((connection, transaction) =>
            connection.ExecuteAsync(
                new CommandDefinition(Sql.RegenerateActionPoint, new 
                {
                    UserId = userId, 
                    NewActionPoint = newActionPoint,
                    LastUpdated = lastUpdated.ToUniversalTime()
                }, transaction: transaction, cancellationToken: ct)
        ), ct);
    }

    public async Task SetActionPointToMaxAsync(int userId, CancellationToken ct)
    {
        await ExecuteAsync((connection, transaction) =>
            connection.ExecuteAsync(
                new CommandDefinition(Sql.SetActionPointToMax, new
                {
                    UserId = userId,
                    LastUpdated = DateTime.UtcNow
                }, transaction: transaction, cancellationToken: ct)
        ), ct);
    }
}