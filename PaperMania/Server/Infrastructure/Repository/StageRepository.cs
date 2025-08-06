using Dapper;
using Server.Application.Port;
using Server.Domain.Entity;

namespace Server.Infrastructure.Repository;

public class StageRepository : RepositoryBase, IStageRepository
{
    public StageRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task CreatePlayerStageDataAsync(int? userId)    
    {
        await using var db = CreateConnection();
        await db.OpenAsync();
        
        var sql = @"
                INSERT INTO paper_mania_stage_data.player_stage_data (id, stage_num, stage_sub_num, is_cleared)
                VALUES (@Id, @StageNum, @SubStageNum, false);
            ";
        
        var data = new List<dynamic>();
        for (int stageNum = 1; stageNum <= 5; stageNum++)
        {
            for (int subNum = 1; subNum <= 5; subNum++)
            {
                data.Add(new PlayerStageData
                {
                    Id = userId,
                    StageNum = stageNum,
                    SubStageNum = subNum
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
            FROM paper_mania_stage_data.player_stage_data
            WHERE id = @Id AND stage_num = @StageNum AND stage_sub_num = @SubStageNum
            LIMIT 1";
        
        var result = await db.QueryFirstOrDefaultAsync<bool?>(sql, new
        {
            Id = data.Id,
            StageNum = data.StageNum,
            SubStageNum = data.SubStageNum
        });

        return result ?? false;
    }

    public async Task UpdateIsClearedAsync(PlayerStageData data)
    {
        await using var db = CreateConnection();
        await db.OpenAsync();

        var sql = @"
            UPDATE paper_mania_stage_data.player_stage_data
            SET is_cleared = @IsCleared
            WHERE id = @Id AND stage_num = @StageNum AND stage_sub_num = @SubStageNum";
        
        await db.ExecuteAsync(sql, new
        {
            Id = data.Id,
            IsCleared = data.IsCleared,
            StageNum = data.StageNum,
            SubStageNum = data.SubStageNum
        });
    }
}