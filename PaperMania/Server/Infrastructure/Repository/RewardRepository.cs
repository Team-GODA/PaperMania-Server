using Dapper;
using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port;
using Server.Domain.Entity;
using Server.Infrastructure.Service;

namespace Server.Infrastructure.Repository;

public class RewardRepository : RepositoryBase, IRewardRepository
{
    private static class Sql
    {
        public const string UpdateCurrency = @"
            UPDATE paper_mania_game_data.player_currency_data
            SET gold = gold + @Gold,
                paper_piece = paper_piece + @PaperPiece
            WHERE user_id = @UserId
            ";

        public const string UpdatePlayerExp = @"
            UPDATE paper_mania_game_data.player_game_data
            SET player_exp = player_exp + @Exp
            WHERE user_id = @UserId
            ";
    }
    
    private readonly StageRewardCache _cache;
    
    public RewardRepository(
        string connectionString,
        StageRewardCache cache,
        IUnitOfWork? unitOfWork = null) 
        : base(connectionString, unitOfWork)
    {
        _cache = cache;
    }

    public StageReward? GetStageReward(int stageNum, int stageSubNum)
    {
        return _cache.GetStageReward(stageNum, stageSubNum);
    }

    public async Task ClaimStageRewardByUserIdAsync(int userId, StageReward reward)
    {
        await ExecuteAsync(async (connection, transaction) =>
        {
            var updatedCurrency = await connection.ExecuteAsync(
                Sql.UpdateCurrency,
                new
                {
                    Gold = reward.Gold,
                    PaperPiece = reward.PaperPiece,
                    UserId = userId
                },
                transaction);

            var updatedExp = await connection.ExecuteAsync(
                Sql.UpdatePlayerExp,
                new
                {
                    Exp = reward.ClearExp,
                    UserId = userId
                },
                transaction);

            if (updatedCurrency == 0)
                throw new RequestException(ErrorStatusCode.NotFound,
                    "PLAYER_CURRENCY_NOT_FOUND",
                    new { UserId = userId });

            if (updatedExp == 0)
                throw new RequestException(ErrorStatusCode.NotFound,
                    "PLAYER_GAME_NOT_FOUND",
                    new { UserId = userId });
        });
    }
}