using Dapper;
using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port;
using Server.Domain.Entity;
using Server.Infrastructure.Service;

namespace Server.Infrastructure.Repository;

public class RewardRepository : RepositoryBase, IRewardRepository
{
    private readonly StageRewardCache _cache;
    
    public RewardRepository(string connectionString, StageRewardCache cache) : base(connectionString)
    {
        _cache = cache;
    }

    public StageReward? GetStageReward(int stageNum, int stageSubNum)
    {
        return _cache.GetStageReward(stageNum, stageSubNum);
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
            WHERE user_id = @UserId";

            var currencyResult  = await db.ExecuteAsync(updateCurrencySql, new
            {
                Gold = reward.Gold,
                PaperPiece = reward.PaperPiece,
                UserId = userId
            }, transaction);
        
            var updateExpSql = @"
            UPDATE paper_mania_game_data.player_game_data
            SET player_exp = player_exp + @ClearExp
            WHERE user_id = @UserId";
        
            var gameResult = await db.ExecuteAsync(updateExpSql, new
            {
                ClearExp = reward.ClearExp,
                UserId = userId
            }, transaction);

            if (currencyResult == 0)
                throw new RequestException(ErrorStatusCode.NotFound, $"user {userId}의 player_currency_data 없음");
            if (gameResult == 0)
                throw new RequestException(ErrorStatusCode.NotFound, $"user {userId}의 player_game_data 없음");
            
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}