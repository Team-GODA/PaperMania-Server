using Dapper;
using Server.Application.Port.Output.Persistence;
using Server.Application.Port.Output.Transaction;
using Server.Infrastructure.Persistence.Model;

namespace Server.Infrastructure.Persistence.Dao;

public class StageRepository : RepositoryBase, IStageRepository
{
    private static class Sql
    {
        public const string GetStageData = @"
            SELECT user_id AS UserId, 
                   stage_num AS StageNum, 
                   stage_sub_num AS StageSubNum
            FROM paper_mania_game_data.player_stage_data
            WHERE user_id = @UserId 
              AND stage_num = @StageNum 
              AND stage_sub_num = @StageSubNum;
        ";
        
        public const string CreateStageData = @"
                INSERT INTO paper_mania_game_data.player_stage_data (user_id, stage_num, stage_sub_num)
                VALUES (@UserId, @StageNum, @StageSubNum);
        ";
    }
    
    public StageRepository(
        string connectionString,
        ITransactionScope? transactionScope = null) 
        : base(connectionString, transactionScope)
    {
    }

    public async Task<PlayerStageData?> FindByUserIdAsync(int userId, int stageNum, int stageSubNum, CancellationToken ct)
    {
        return await QueryAsync(connection =>
            connection.QueryFirstOrDefaultAsync<PlayerStageData>(
                new CommandDefinition(Sql.GetStageData, new { UserId = userId, StageNum = stageNum, StageSubNum = stageSubNum }, cancellationToken: ct)
            ), ct);
    }

    public async Task CreateAsync(PlayerStageData data, CancellationToken ct)
    {
        await ExecuteAsync((connection, transaction) =>
            connection.ExecuteAsync(
                new CommandDefinition(Sql.CreateStageData, new { UserId = data.UserId, StageNum = data.StageNum, StageSubNum = data.StageSubNum }, transaction: transaction, cancellationToken: ct)
        ), ct);
    }
}