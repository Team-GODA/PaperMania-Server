using Dapper;
using Server.Application.Port;
using Server.Domain.Entity;

namespace Server.Infrastructure.Repository;

public class CurrencyRepository : RepositoryBase, ICurrencyRepository
{
    public CurrencyRepository(string connectionString) : base(connectionString)
    {
    }
    
    public async Task AddPlayerGoodsDataByUserIdAsync(int? userId)
    {
        await using var db = CreateConnection();
        await db.OpenAsync();

        var sql = @"
            INSERT INTO paper_mania_game_data.player_currency_data (id)
            VALUES (@UserId)";

        await db.ExecuteAsync(sql, new { UserId = userId });
    }

    public async Task<PlayerCurrencyData> GetPlayerGoodsDataByUserIdAsync(int? userId)
    {
        await using var db = CreateConnection();
        await db.OpenAsync();

        var sql = @"
            SELECT id AS Id, action_point AS ActionPoint, action_point_max AS MaxActionPoint, 
                gold AS Gold, paper_piece AS PaperPiece, last_action_point_updated AS LastActionPointUpdated
            FROM paper_mania_game_data.player_currency_data
            WHERE id = @Id";
        
        var result = await db.QueryFirstOrDefaultAsync<PlayerCurrencyData>(sql, new { Id = userId });
        return result ?? throw new InvalidOperationException($"플레이어 재화 데이터 NULL : Id : {userId}");
    }

    public async Task UpdatePlayerGoodsDataAsync(PlayerCurrencyData data)
    {
        await using var db = CreateConnection();
        await db.OpenAsync();

        var sql = @"
            UPDATE paper_mania_game_data.player_currency_data
            SET action_point = @ActionPoint,
                action_point_max = @MaxActionPoint,
                last_action_point_updated = @LastActionPointUpdated,
                gold = @Gold,
                paper_piece = @PaperPiece
            WHERE id = @Id";

        await db.ExecuteAsync(sql, data);
    }
}