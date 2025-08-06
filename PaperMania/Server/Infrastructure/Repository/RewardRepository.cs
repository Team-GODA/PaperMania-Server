using Dapper;
using Server.Application.Port;
using Server.Domain.Entity;

namespace Server.Infrastructure.Repository;

public class RewardRepository : RepositoryBase, IRewardRepository
{
    public RewardRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<StageReward?> GetStageRewardAsync(int stageNum, int stageSubNum)
    {
        await using var db = CreateConnection();
        await db.OpenAsync();

        var sql = @"
            SELECT stage_num AS StageNum, 
               stage_sub_num AS SubStageNum, 
               clear_paper_piece AS PaperPiece, 
               clear_gold AS Gold, 
               clear_exp AS ClearExp
        FROM paper_mania_stage_data.stage_reward 
        WHERE stage_num = @StageNum 
          AND stage_sub_num = @SubStageNum";

        var result = await db.QueryFirstOrDefaultAsync<StageReward>(sql, new
        {
            StageNum = stageNum,
            SubStageNum = stageSubNum
        });
        
        return result;
    }

    public async Task ClaimStageRewardByUserIdAsync(int? userId, StageReward reward)
    {
        await using var db = CreateConnection();
        await db.OpenAsync();

        await using var transaction = await db.BeginTransactionAsync();

        try
        {
            var updateCurrencySql = @"
            UPDATE paper_mania_game_data.player_currency_data
            SET gold = gold + @Gold,
                paper_piece = paper_piece + @PaperPiece
            WHERE id = @UserId";

            var currencyResult  = await db.ExecuteAsync(updateCurrencySql, new
            {
                Gold = reward.Gold,
                PaperPiece = reward.PaperPiece,
                UserId = userId
            }, transaction);
        
            var updateExpSql = @"
            UPDATE paper_mania_game_data.player_game_data
            SET player_exp = player_exp + @ClearExp
            WHERE id = @UserId";
        
            var gameResult = await db.ExecuteAsync(updateExpSql, new
            {
                ClearExp = reward.ClearExp,
                UserId = userId
            }, transaction);
        
            if (currencyResult == 0 || gameResult == 0)
                throw new InvalidOperationException($"UserId {userId}에 해당하는 데이터가 없습니다.");
        
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}