using Dapper;
using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port;
using Server.Domain.Entity;
using Server.Infrastructure.Service;

namespace Server.Infrastructure.Repository;

public class CharacterRepository : RepositoryBase, ICharacterRepository
{
    private readonly CharacterDataCache _cache;

    public CharacterRepository(string connectionString, CharacterDataCache cache) : base(connectionString)
    {
        _cache = cache;
    }

    public async Task<IEnumerable<PlayerCharacterData>> GetPlayerCharactersDataByUserIdAsync(int userId)
    {
        await using var db = CreateConnection();
        await db.OpenAsync();

        var sql = @"
            SELECT PC.user_id AS UserId, PC.character_id AS CharacterId,
                   PC.character_level AS CharacterLevel,
                   PC.normal_skill_level AS NormalSkillLevel,
                   PC.epic_skill_level AS EpicSkillLevel,
                   PP.character_piece AS PieceAmount
            FROM paper_mania_game_data.player_character_data PC
            JOIN paper_mania_game_data.player_character_piece_data PP
                ON PC.user_id = PP.user_id AND PC.character_id = PP.character_id
            WHERE user_id = @UserId
        ";

        var playerCharacters = (await db.QueryAsync<PlayerCharacterData>(sql, new { UserId = userId })).ToList();

        foreach (var pc in playerCharacters)
        {
            var baseData = _cache.GetCharacter(pc.Data.CharacterId);
            if (baseData == null)
                throw new RequestException(ErrorStatusCode.NotFound, "CHARACTER_NOT_FOUND",
                    new { CharacterId = pc.Data.CharacterId });

            pc.Data = baseData;
        }

        return playerCharacters;
    }

    public async Task<PlayerCharacterData> AddPlayerCharacterDataByUserIdAsync(PlayerCharacterData data)
    {
        await using var db = CreateConnection();
        await db.OpenAsync();

        var insertCharacterSql = @"
            INSERT INTO paper_mania_game_data.player_character_data
            (user_id, character_id, character_level, normal_skill_level, epic_skill_level)
            VALUES (@UserId, @CharacterId, 1, 1, 1);
        ";

        var insertPieceSql = @"
            INSERT INTO paper_mania_game_data.player_character_piece_data
            (user_id, character_id, character_piece)
            VALUES (@UserId, @CharacterId, 0);
        ";

        var param = new { UserId = data.UserId, CharacterId = data.Data.CharacterId };
        await using var transaction = await db.BeginTransactionAsync();
        
        await db.ExecuteAsync(insertCharacterSql, param, transaction);
        await db.ExecuteAsync(insertPieceSql, param, transaction);

        await transaction.CommitAsync();
        
        var baseData = _cache.GetCharacter(data.Data.CharacterId);
        if (baseData == null)
            throw new RequestException(ErrorStatusCode.NotFound, "CHARACTER_NOT_FOUND",
                new { CharacterId = data.Data.CharacterId });

        data.Data = baseData;
        return data;
    }

    public async Task<bool> HasCharacterAsync(int userId, string characterId)
    {
        await using var db = CreateConnection();
        await db.OpenAsync();

        var sql = @"
            SELECT EXISTS (
                SELECT 1
                FROM paper_mania_game_data.player_character_data
                WHERE user_id = @UserId
                  AND character_id = @CharacterId
            );
        ";

        return await db.QuerySingleAsync<bool>(sql, new { UserId = userId, CharacterId = characterId });
    }

    public async Task AddCharacterPiecesAsync(int userId, string characterId, int amount)
    {
        await using var db = CreateConnection();
        await db.OpenAsync();

        var sql = @"
            UPDATE paper_mania_game_data.player_character_piece_data
            SET character_piece = character_piece + @Amount
            WHERE user_id = @UserId AND character_id = @CharacterId
        ";

        await db.ExecuteAsync(sql, new { Amount = amount, UserId = userId, CharacterId = characterId });
    }
}