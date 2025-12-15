using Dapper;
using Server.Application.Port;
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
            WHERE id = @UserId
            ";
        
        public const string UpdatePlayerCurrencyData = @"
            UPDATE paper_mania_game_data.player_currency_data
            SET action_point = @ActionPoint,
                action_point_max = @MaxActionPoint,
                last_action_point_updated = @LastActionPointUpdated,
                gold = @Gold,
                paper_piece = @PaperPiece
            WHERE id = @UserId
            ";
    }
    
    public CurrencyRepository(
        string connectionString,
        IUnitOfWork? unitOfWork = null) 
        : base(connectionString, unitOfWork)
    {
    }
    
    public async Task AddPlayerCurrencyDataByUserIdAsync(int? userId)
    {
        await ExecuteAsync(async (connection, transaction) =>
            await connection.ExecuteAsync(
                Sql.AddPlayerCurrencyData,
                new { UserId = userId },
                transaction));
    }

    public async Task<PlayerCurrencyData> FindPlayerCurrencyDataByUserIdAsync(int? userId)
    {
        var result = await ExecuteAsync(async (connection, transaction) =>
            await connection.QueryFirstOrDefaultAsync<PlayerCurrencyData>(
                Sql.GetPlayerCurrencyData,
                new { UserId = userId },
                transaction));
        
        return result ?? throw new InvalidOperationException($"플레이어 재화 데이터 NULL : UserId : {userId}");
    }

    public async Task UpdatePlayerCurrencyDataAsync(PlayerCurrencyData data)
    {
        await ExecuteAsync(async (connection, transaction) =>
            await connection.ExecuteAsync(
                Sql.UpdatePlayerCurrencyData,
                data,
                transaction));
    }
}