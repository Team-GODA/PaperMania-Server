using Dapper;
using Server.Application.Port;
using Server.Domain.Entity;

namespace Server.Infrastructure.Repository;

public class StageRepository : RepositoryBase, IStageRepository
{
    private static class Sql
    {
        public const string CreateStageData = @"
                INSERT INTO paper_mania_game_data.player_stage_data (user_id, stage_num, stage_sub_num, is_cleared)
                VALUES (@UserId, @StageNum, @StageSubNum, false);
        ";
        
        public const string CheckIsCleared = @"
            SELECT is_cleared AS IsCleared
            FROM paper_mania_game_data.player_stage_data
            WHERE user_id = @UserId AND stage_num = @StageNum AND stage_sub_num = @StageSubNum
            LIMIT 1
            ";
        
        public const string UpdateIsCleared = @"
            UPDATE paper_mania_game_data.player_stage_data
            SET is_cleared = @IsCleared
            WHERE user_id = @UserId AND stage_num = @StageNum AND stage_sub_num = @StageSubNum
            ";
    }
    
    private const int MaxStageNum = 5;
    private const int MaxSubStageNum = 5;
    
    public StageRepository(
        string connectionString,
        ITransactionScope? transactionScope = null) 
        : base(connectionString, transactionScope)
    {
    }

    public async Task CreatePlayerStageDataAsync(int? userId)    
    {
        var data = new List<object>(MaxStageNum * MaxSubStageNum);
        
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

        await ExecuteAsync(async (connection, transaction) =>
            await connection.ExecuteAsync(Sql.CreateStageData, data, transaction));
    }

    public async Task<bool> IsClearedStageAsync(PlayerStageData data)
    {
        return await ExecuteAsync(async (connection, transaction) =>
        {
            var result = await connection.QueryFirstOrDefaultAsync<bool?>(
                Sql.CheckIsCleared,
                new
                {
                    UserId = data.UserId,
                    StageNum = data.StageNum,
                    StageSubNum = data.StageSubNum
                },
                transaction);

            return result ?? false;
        });
    }

    public async Task UpdateIsClearedAsync(PlayerStageData data)
    {
        await ExecuteAsync(async (connection, transaction) =>
            await connection.ExecuteAsync(
                Sql.UpdateIsCleared, 
                new
                {
                    UserId = data.UserId,
                    IsCleared = data.IsCleared,
                    StageNum = data.StageNum,
                    StageSubNum = data.StageSubNum
                },
                transaction));
    }
}