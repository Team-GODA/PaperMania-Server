using Dapper;
using Server.Application.Port;
using Server.Application.Port.Out.Infrastructure;
using Server.Application.Port.Out.Persistence;
using Server.Domain.Entity;

namespace Server.Infrastructure.Repository;

public class PlayerRepository : RepositoryBase ,IPlayerRepository
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
        
        public const string GetPlayerDataByName = @"
            SELECT user_id AS UserId, player_name AS PlayerName, player_exp AS PlayerExp, player_level AS PlayerLevel
            FROM paper_mania_game_data.player_game_data
            WHERE player_name = @PlayerName
            LIMIT 1
            ";
        
        public const string UpdatePlayerData = @"
            UPDATE paper_mania_game_data.player_game_data
            SET
                player_name = @PlayerName,
                player_level = @PlayerLevel,
                player_exp = @PlayerExp
            WHERE user_id = @UserId;
            ";
    }

    public PlayerRepository(
        string connectionString,
        ITransactionScope? transactionScope = null) 
        : base(connectionString, transactionScope)
    {
    }

    public async Task<PlayerGameData?> FindByUserIdAsync(long userId)
    {
        return await QueryAsync(connection =>
            connection.QueryFirstOrDefaultAsync<PlayerGameData>(
                Sql.GetPlayerDataById,
                new { UserId = userId }
            ));
    }

    public async Task<PlayerGameData?> FindByNameAsync(string playerName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(playerName);

        return await QueryAsync(connection =>
            connection.QueryFirstOrDefaultAsync<PlayerGameData>(
                Sql.GetPlayerDataByName,
                new { PlayerName = playerName }
            ));
    }

    public async Task CreateAsync(PlayerGameData player)
    {
        ArgumentNullException.ThrowIfNull(player);

        await ExecuteAsync( (connection, transaction) =>
            connection.ExecuteAsync(
                Sql.AddPlayerData,
                new { UserId = player.UserId, PlayerName = player.PlayerName },
                transaction)
        );
    }

    public async Task UpdateAsync(PlayerGameData player)
    {
        ArgumentNullException.ThrowIfNull(player);
        
        var rows = await ExecuteAsync((connection, transaction) =>
            connection.ExecuteAsync(
                Sql.UpdatePlayerData,
                new 
                {
                    UserId = player.UserId,
                    PlayerName = player.PlayerName, 
                    PlayerLevel = player.PlayerLevel, 
                    PlayerExp = player.PlayerExp 
                },
                transaction)
        );
        
        if (rows == 0)
            throw new InvalidOperationException(
                $"PLAYER_NOT_FOUND: userId={player.UserId}"
            );
    }
}