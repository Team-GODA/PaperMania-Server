using Dapper;
using Server.Application.Port;
using Server.Domain.Entity;

namespace Server.Infrastructure.Repository;

public class DataRepository : RepositoryBase, IDataRepository
{
    public DataRepository(string connectionString) : base(connectionString)
    {
    }
    
    public async Task<PlayerGameData?> ExistsPlayerNameAsync(string playerName)
    {
        await using var db = CreateConnection();
        await db.OpenAsync();
        
        var sql = @"
            SELECT user_id, player_name AS PlayerName, player_exp AS PlayerExp, player_level AS PlayerLevel
            FROM paper_mania_game_data.player_game_data
            WHERE player_name = @PlayerName
            LIMIT 1";
        
        return await db.QueryFirstOrDefaultAsync<PlayerGameData>(sql, new { PlayerName = playerName });
    }

    public async Task AddPlayerDataAsync(int? userId, string playerName)
    {
        await using var db = CreateConnection();
        await db.OpenAsync();

        var sql = @"
        INSERT INTO paper_mania_game_data.player_game_data (user_id, player_name)
        VALUES (@UserId, @PlayerName)";

        await db.ExecuteAsync(sql, new { UserId = userId, PlayerName = playerName });
    }

    public async Task<PlayerGameData?> GetPlayerDataByIdAsync(int? userId)
    {
        await using var db = CreateConnection();
        await db.OpenAsync();
        
        var sql = @"
            SELECT user_id AS UserId, player_name AS PlayerName, player_exp AS PlayerExp, player_level AS PlayerLevel
            FROM paper_mania_game_data.player_game_data
            WHERE user_id = @UserId
            LIMIT 1";
        
        return await db.QueryFirstOrDefaultAsync<PlayerGameData>(sql, new { UserId = userId });
    }

    public async Task<PlayerGameData?> UpdatePlayerLevelAsync(int? userId, int newLevel, int newExp)
    {
        await using var db = CreateConnection();
        await db.OpenAsync();
        
        var sql = @"
            UPDATE paper_mania_game_data.player_game_data
            SET player_level = @Level, player_exp = @Exp
            WHERE user_id = @UserId
            RETURNING user_id, player_name AS PlayerName, player_exp AS PlayerExp, player_level AS PlayerLevel;
            ";

        return await db.QueryFirstOrDefaultAsync<PlayerGameData>(sql, new
        {
            Level = newLevel,
            Exp = newExp,
            UserId = userId
        });
    }

    public async Task<LevelDefinition?> GetLevelDataAsync(int currentLevel)
    {
        await using var db = CreateConnection();
        await db.OpenAsync();

        var sql = @"
            SELECT level AS Level, max_exp AS MaxExp, max_action_point AS MaxActionPoint
            FROM paper_mania_game_data.level_definition
            WHERE level = @CurrentLevel";
        
        return await db.QueryFirstOrDefaultAsync<LevelDefinition>(sql, new { CurrentLevel = currentLevel });
    }

    public async Task RenamePlayerNameAsync(int? userId, string newPlayerName)
    {
        await using var db = CreateConnection();
        await db.OpenAsync();
        
        var sql = @"
            UPDATE paper_mania_game_data.player_game_data
            SET player_name = @PlayerName
            WHERE user_id = @UserId
            ";
        
        await db.ExecuteAsync(sql, new { PlayerName = newPlayerName, UserId = userId });
    }
}