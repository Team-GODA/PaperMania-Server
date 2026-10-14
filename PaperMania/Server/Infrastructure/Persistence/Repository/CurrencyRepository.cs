using Dapper;
using Server.Application.Port.Output.Persistence;
using Server.Application.Port.Output.Transaction;
using Server.Domain.Entity;
using Server.Infrastructure.Persistence.Model;

namespace Server.Infrastructure.Persistence.Repository;

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

        // 트랜잭션 안의 조회 -> 갱신 사이에 다른 요청이 같은 행을 덮어쓰지 못하도록 커밋까지 행 잠금
        public const string GetPlayerCurrencyDataForUpdate = GetPlayerCurrencyData + "FOR UPDATE";

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

    public async Task<CurrencyData?> FindByUserIdAsync(int userId, CancellationToken ct)
    {
        var data = await QueryAsync(connection =>
            connection.QueryFirstOrDefaultAsync<PlayerCurrencyData>(
                new CommandDefinition(InTransaction ? Sql.GetPlayerCurrencyDataForUpdate : Sql.GetPlayerCurrencyData,
                    new { UserId = userId }, cancellationToken: ct)
            ), ct);

        return data == null
            ? null
            : new CurrencyData(
                data.UserId,
                data.ActionPoint,
                data.MaxActionPoint,
                data.Gold,
                data.PaperPiece,
                data.LastActionPointUpdated);
    }

    public async Task UpdateAsync(CurrencyData currencyData, CancellationToken ct)
    {
        var data = new PlayerCurrencyData(
            currencyData.UserId,
            currencyData.ActionPoint,
            currencyData.MaxActionPoint,
            currencyData.Gold,
            currencyData.PaperPiece,
            currencyData.LastActionPointUpdated
        );

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