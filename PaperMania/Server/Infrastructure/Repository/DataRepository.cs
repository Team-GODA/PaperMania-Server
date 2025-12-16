using Dapper;
using Server.Application.Port;
using Server.Domain.Entity;

namespace Server.Infrastructure.Repository;

public class DataRepository : RepositoryBase, IDataRepository
{
    private static class Sql
    {
        public const string ExistsPlayerName = @"
            SELECT user_id, player_name AS PlayerName, player_exp AS PlayerExp, player_level AS PlayerLevel
            FROM paper_mania_game_data.player_game_data
            WHERE player_name = @PlayerName
            LIMIT 1
            ";
        
        public const string AddPlayerData = @"
        INSERT INTO paper_mania_game_data.player_game_data (user_id, player_name)
        VALUES (@UserId, @PlayerName)
        ";

        public const string GetPlayerDataById = @"
            SELECT user_id AS UserId, player_name AS PlayerName, player_exp AS PlayerExp, player_level AS PlayerLevel
            FROM paper_mania_game_data.player_game_data
            WHERE user_id = @UserId
            LIMIT 1
            ";

        public const string UpdatePlayerLevel = @"
            UPDATE paper_mania_game_data.player_game_data
            SET player_level = @Level, player_exp = @Exp
            WHERE user_id = @UserId
            RETURNING user_id, player_name AS PlayerName, player_exp AS PlayerExp, player_level AS PlayerLevel;
            ";
        
        public const string GetLevelData = @"
            SELECT level AS Level, max_exp AS MaxExp, max_action_point AS MaxActionPoint
            FROM paper_mania_game_data.level_definition
            WHERE level = @CurrentLevel
            LIMIT 1
            ";
        
        public const string RenamePlayerName = @"
            UPDATE paper_mania_game_data.player_game_data
            SET player_name = @PlayerName
            WHERE user_id = @UserId
            ";
    }
    
    public DataRepository(
        string connectionString, 
        ITransactionScope? transactionScope = null) 
        : base(connectionString, transactionScope)
    {
    }
    
    public async Task<PlayerGameData?> ExistsPlayerNameAsync(string playerName)
    {
        return await ExecuteAsync(async (connection, transaction) =>
            await connection.QueryFirstOrDefaultAsync<PlayerGameData>(
                Sql.ExistsPlayerName,
                new { PlayerName = playerName },
                transaction));
    }

    public async Task CreateDataAsync(int? userId, string playerName)
    {
        await ExecuteAsync(async (connection, transaction) =>
            await connection.ExecuteAsync(
                Sql.AddPlayerData,
                new { UserId = userId, PlayerName = playerName },
                transaction));
    }

    public async Task<PlayerGameData?> FindByUserIdAsync(int? userId)
    {
        return await ExecuteAsync(async (connection, transaction) =>
            await connection.QueryFirstOrDefaultAsync<PlayerGameData>(
                Sql.GetPlayerDataById,
                new { UserId = userId },
                transaction));
    }

    public async Task<PlayerGameData?> UpdatePlayerLevelAsync(int? userId, int newLevel, int newExp)
    {
        return await ExecuteAsync(async (connection, transaction) =>
            await connection.QueryFirstOrDefaultAsync<PlayerGameData>(
                Sql.UpdatePlayerLevel,
                new
                {
                    Level = newLevel,
                    Exp = newExp,
                    UserId = userId
                },
                transaction));
    }

    public async Task<LevelDefinition?> FindLevelDataAsync(int currentLevel)
    {
        return await ExecuteAsync(async (connection, transaction) =>
            await connection.QueryFirstOrDefaultAsync<LevelDefinition>(
                Sql.GetLevelData,
                new { CurrentLevel = currentLevel },
                transaction));
    }

    public async Task RenamePlayerNameAsync(int? userId, string newPlayerName)
    {
        await ExecuteAsync(async (connection, transaction) =>
            await connection.ExecuteAsync(
                Sql.RenamePlayerName,
                new { PlayerName = newPlayerName, UserId = userId },
                transaction));
    }
}