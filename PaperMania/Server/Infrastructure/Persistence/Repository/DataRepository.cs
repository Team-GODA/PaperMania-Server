using Dapper;
using Server.Application.Port.Output.Persistence;
using Server.Application.Port.Output.Transaction;
using Server.Domain.Entity;
using Server.Infrastructure.Persistence.Model;
using Server.Infrastructure.StaticData.Model;

namespace Server.Infrastructure.Persistence.Repository;

public class DataRepository : RepositoryBase, IDataRepository
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

        public const string UpdatePlayerData = @"
            UPDATE paper_mania_game_data.player_game_data
            SET player_level = @Level,
                player_exp = @Exp,
                player_name = @Name
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
    
    public DataRepository(
        string connectionString, 
        ITransactionScope? transactionScope = null) 
        : base(connectionString, transactionScope)
    {
    }
    
    private static PlayerGameData MapToModel(GameData entity) => 
        new(entity.UserId, entity.Name, entity.Exp, entity.Level);
    
    private static GameData? MapToEntity(PlayerGameData? data) => 
        data == null ? null : new GameData(data.UserId, data.Name, data.Exp, data.Level);
    
    public async Task<GameData?> ExistsPlayerNameAsync(string playerName, CancellationToken ct)
    {
        var data = await ExecuteAsync((connection, transaction) =>
             connection.QueryFirstOrDefaultAsync<PlayerGameData>(
                new CommandDefinition(Sql.ExistsPlayerName, new { Name = playerName }, transaction: transaction, cancellationToken: ct)
             ), ct);
        
        return  MapToEntity(data);
    }

    public async Task CreateAsync(GameData player, CancellationToken ct)
    {
        await ExecuteAsync((connection, transaction) =>
            connection.ExecuteAsync(
                new CommandDefinition(Sql.AddPlayerData, new { UserId = player.UserId, Name = player.Name }, transaction: transaction, cancellationToken: ct)
        ), ct);
    }

    public async Task<GameData?> FindByUserIdAsync(int? userId, CancellationToken ct)
    {
        var data =  await QueryAsync(connection =>
             connection.QueryFirstOrDefaultAsync<PlayerGameData>(
                new CommandDefinition(Sql.GetPlayerDataById, new { UserId = userId }, cancellationToken: ct)
             ), ct);
        
        return  MapToEntity(data);
    }

    public async Task UpdateAsync(GameData data, CancellationToken ct)
    {
        await ExecuteAsync((conn, trans) => conn.ExecuteAsync(
            new CommandDefinition(Sql.UpdatePlayerData, MapToModel(data), transaction: trans, cancellationToken: ct)
        ), ct);
    }

    public async Task<LevelDefinition?> FindLevelDataAsync(int currentLevel, CancellationToken ct)
    {
        return await QueryAsync(connection =>
             connection.QueryFirstOrDefaultAsync<LevelDefinition>(
                new CommandDefinition(Sql.GetLevelData, new { Level = currentLevel }, cancellationToken: ct)
             ), ct);
    }

    public async Task RenamePlayerNameAsync(int? userId, string newPlayerName, CancellationToken ct)
    {
        await ExecuteAsync((connection, transaction) => 
            connection.ExecuteAsync(
                new CommandDefinition(Sql.RenamePlayerName, new { Name = newPlayerName, UserId = userId }, transaction: transaction, cancellationToken: ct)
            ), ct);
    }
}