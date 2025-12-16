using Dapper;
using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port;
using Server.Domain.Entity;
using Server.Infrastructure.Service;

namespace Server.Infrastructure.Repository;

public class CharacterRepository : RepositoryBase, ICharacterRepository
{
    private static class Sql
    {
        public const string GetPlayerCharactersData = @"
            SELECT PC.user_id AS UserId, PC.character_id AS CharacterId,
                   PC.character_level AS CharacterLevel,
                   PC.normal_skill_level AS NormalSkillLevel,
                   PC.epic_skill_level AS EpicSkillLevel,
                   PP.character_piece AS PieceAmount
            FROM paper_mania_game_data.player_character_data PC
            JOIN paper_mania_game_data.player_character_piece_data PP
                ON PC.user_id = PP.user_id AND PC.character_id = PP.character_id
            WHERE PC.user_id = @UserId
            ";
        
        public const string InsertCharacter = @"
            INSERT INTO paper_mania_game_data.player_character_data
            (user_id, character_id, character_level, normal_skill_level, epic_skill_level)
            VALUES (@UserId, @CharacterId, 1, 1, 1)
            ";
        
        public const string InsertCharacterPiece = @"
            INSERT INTO paper_mania_game_data.player_character_piece_data
            (user_id, character_id, character_piece)
            VALUES (@UserId, @CharacterId, 0)
            ";
        
        public const string HasCharacter = @"
            SELECT EXISTS (
                SELECT 1
                FROM paper_mania_game_data.player_character_data
                WHERE user_id = @UserId AND character_id = @CharacterId
            )
            ";
        
        public const string AddCharacterPieces = @"
            UPDATE paper_mania_game_data.player_character_piece_data
            SET character_piece = character_piece + @Amount
            WHERE user_id = @UserId AND character_id = @CharacterId
            ";
    }
    
    private readonly CharacterDataCache _cache;

    public CharacterRepository(
        string connectionString,
        CharacterDataCache cache,
        ITransactionScope? transactionScope = null)
        : base(connectionString, transactionScope)
    {
        _cache = cache;
    }

    public async Task<IEnumerable<PlayerCharacterData>> GetPlayerCharactersDataByUserIdAsync(int userId)
    {
        var playerCharacters = (await ExecuteAsync(async (connection, transaction) =>
            await connection.QueryAsync<PlayerCharacterData>(
                Sql.GetPlayerCharactersData,
                new { UserId = userId },
                transaction))).ToList();
        
        foreach (var pc in playerCharacters)
        {
            var baseData = _cache.GetCharacter(pc.Data.CharacterId);
            if (baseData == null)
                throw new RequestException(ErrorStatusCode.NotFound, 
                    "CHARACTER_NOT_FOUND",
                    new { CharacterId = pc.Data.CharacterId });

            pc.Data = baseData;
        }
        
        return playerCharacters;
    }

    public async Task AddPlayerCharacterDataByUserIdAsync(PlayerCharacterData data)
    {
        await ExecuteAsync(async (connection, transaction) =>
        {
            var character = await connection.QuerySingleAsync<PlayerCharacterData>(
                Sql.InsertCharacter,
                new
                {
                    UserId = data.UserId,
                    CharacterId = data.Data.CharacterId,
                    Level = data.CharacterLevel,
                    NormalSkillLevel = data.NormalSkillLevel,
                    EpicSkillLevel = data.EpicSkillLevel
                },
                transaction);

            await connection.ExecuteAsync(
                Sql.InsertCharacterPiece,
                new
                {
                    UserId = data.UserId,
                    CharacterId = data.Data.CharacterId,
                    PieceAmount = data.PieceAmount
                },
                transaction);
        });
    }

    public async Task<bool> HasCharacterAsync(int userId, string characterId)
    {
        return await ExecuteAsync(async (connection, transaction) =>
            await connection.QuerySingleAsync<bool>(
                Sql.HasCharacter, 
                new { UserId = userId, CharacterId = characterId }, 
                transaction));
    }

    public async Task AddCharacterPiecesAsync(int userId, string characterId, int amount)
    {
        await ExecuteAsync(async (connection, transaction) =>
            await connection.ExecuteAsync(
                Sql.AddCharacterPieces,
                new
                {
                    Amount = amount,
                    UserId = userId,
                    CharacterId = characterId
                },
                transaction));
    }
}