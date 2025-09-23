using Dapper;
using Server.Application.Port;
using Server.Domain.Entity;

namespace Server.Infrastructure.Repository;

public class StageRepository : RepositoryBase, IStageRepository
{
    private const int MaxStageNum = 5;
    private const int MaxSubStageNum = 5;
    
    public StageRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task CreatePlayerStageDataAsync(int? userId)    
    {
        await using var db = CreateConnection();
        await db.OpenAsync();
        
        var sql = @"
                INSERT INTO paper_mania_game_data.player_stage_data (user_id, stage_num, stage_sub_num, is_cleared)
                VALUES (@UserId, @StageNum, @StageSubNum, false);
        ";
        
        var data = new List<dynamic>();
        for (int stageNum = 1; stageNum <= MaxStageNum; stageNum++)
        {
            for (int subNum = 1; subNum <= MaxSubStageNum; subNum++)
            {
                data.Add(new PlayerStageData
                {
                    UserId = userId,
                    StageNum = stageNum,
                    StageSubNum = subNum
                });
            }
        }
        
        await db.ExecuteAsync(sql, data);
    }

    public async Task<bool> IsClearedStageAsync(PlayerStageData data)
    {
        await using var db = CreateConnection();
        await db.OpenAsync();
        
        var sql = @"
            SELECT is_cleared AS IsCleared
            FROM paper_mania_game_data.player_stage_data
            WHERE user_id = @UserId AND stage_num = @StageNum AND stage_sub_num = @StageSubNum
            LIMIT 1";
        
        var result = await db.QueryFirstOrDefaultAsync<bool?>(sql, new
        {
            UserId = data.UserId,
            StageNum = data.StageNum,
            StageSubNum = data.StageSubNum
        });

        return result ?? false;
    }

    public async Task UpdateIsClearedAsync(PlayerStageData data)
    {
        await using var db = CreateConnection();
        await db.OpenAsync();

        var sql = @"
            UPDATE paper_mania_game_data.player_stage_data
            SET is_cleared = @IsCleared
            WHERE user_id = @UserId AND stage_num = @StageNum AND stage_sub_num = @StageSubNum";
        
        await db.ExecuteAsync(sql, new
        {
            UserId = data.UserId,
            IsCleared = data.IsCleared,
            StageNum = data.StageNum,
            StageSubNum = data.StageSubNum
        });
    }
}