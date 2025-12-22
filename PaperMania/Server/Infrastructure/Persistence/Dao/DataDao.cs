using Dapper;
using Server.Application.Port.Output.Infrastructure;
using Server.Application.Port.Output.Persistence;
using Server.Infrastructure.Persistence.Model;
using Server.Infrastructure.StaticData;
using Server.Infrastructure.StaticData.Model;

namespace Server.Infrastructure.Persistence.Dao;

public class DataDao : DaoBase, IDataDao
{
    private static class Sql
    {
        public const string ExistsPlayerName = @"
            SELECT user_id, player_name AS Name, player_exp AS Exp, player_level AS Level
            FROM paper_mania_game_data.player_game_data
            WHERE player_name = @PlayerName
            LIMIT 1
            ";
        
        public const string AddPlayerData = @"
        INSERT INTO paper_mania_game_data.player_game_data (user_id, player_name)
        VALUES (@UserId, @Name)
        ";

        public const string GetPlayerDataById = @"
            SELECT user_id AS UserId, player_name AS Name, player_exp AS Exp, player_level AS Level
            FROM paper_mania_game_data.player_game_data
            WHERE user_id = @UserId
            LIMIT 1
            ";

        public const string UpdatePlayerLevel = @"
            UPDATE paper_mania_game_data.player_game_data
            SET player_level = @Level, player_exp = @Exp
            WHERE user_id = @UserId
            RETURNING user_id, player_name AS Name, player_exp AS Exp, player_level AS Level;
            ";
        
        public const string GetLevelData = @"
            SELECT level AS Level, max_exp AS MaxExp, max_action_point AS MaxActionPoint
            FROM paper_mania_game_data.level_definition
            WHERE level = @CurrentLevel
            LIMIT 1
            ";
        
        public const string RenamePlayerName = @"
            UPDATE paper_mania_game_data.player_game_data
            SET player_name = @Name
            WHERE user_id = @UserId
            ";
    }
    
    public DataDao(
        string connectionString, 
        ITransactionScope? transactionScope = null) 
        : base(connectionString, transactionScope)
    {
    }
    
    public async Task<PlayerGameData?> ExistsPlayerNameAsync(string playerName)
    {
        return await ExecuteAsync( (connection, transaction) =>
             connection.QueryFirstOrDefaultAsync<PlayerGameData>(
                Sql.ExistsPlayerName,
                new { Name = playerName },
                transaction)
             );
    }

    public async Task CreateAsync(PlayerGameData player)
    {
        await ExecuteAsync( (connection, transaction) =>
            connection.ExecuteAsync(
                Sql.AddPlayerData,
                new { UserId = player.UserId, Name = player.Name },
                transaction)
        );
    }

    public async Task<PlayerGameData?> FindByUserIdAsync(int? userId)
    {
        return await QueryAsync(connection =>
             connection.QueryFirstOrDefaultAsync<PlayerGameData>(
                Sql.GetPlayerDataById,
                new { UserId = userId }
                )
             );
    }

    public async Task<PlayerGameData?> UpdatePlayerLevelAsync(int? userId, int newLevel, int newExp)
    {
        return await ExecuteAsync( (connection, transaction) =>
             connection.QueryFirstOrDefaultAsync<PlayerGameData>(
                Sql.UpdatePlayerLevel,
                new
                {
                    Level = newLevel,
                    Exp = newExp,
                    UserId = userId
                },
                transaction)
             );
    }

    public async Task<LevelDefinition?> FindLevelDataAsync(int currentLevel)
    {
        return await QueryAsync( connection =>
             connection.QueryFirstOrDefaultAsync<LevelDefinition>(
                Sql.GetLevelData,
                new { Level = currentLevel }
                )
             );
    }

    public async Task RenamePlayerNameAsync(int? userId, string newPlayerName)
    {
        await ExecuteAsync( (connection, transaction) => 
            connection.ExecuteAsync(
                Sql.RenamePlayerName,
                new { Name = newPlayerName, UserId = userId },
                transaction)
            );
    }
}