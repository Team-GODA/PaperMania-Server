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

    public async Task<IEnumerable<PlayerCharacterData>> GetPlayerCharactersDataByUserIdAsync(int? userId)
    {
        if (userId == null)
            throw new RequestException(ErrorStatusCode.NotFound, "USER_NOT_FOUND",
                new { UserId = userId });

        await using var db = CreateConnection();
        await db.OpenAsync();

        var sql = @"
            SELECT user_id AS UserId, character_id AS CharacterId,
                   character_level AS CharacterLevel,
                   normal_skill_level AS NormalSkillLevel,
                   epic_skill_level AS EpicSkillLevel,
                   character_pieces AS PieceAmount
            FROM paper_mania_game_data.player_character_data
            WHERE user_id = @UserId
        ";

        var playerCharacters = (await db.QueryAsync<PlayerCharacterData>(sql, new { UserId = userId })).ToList();

        foreach (var pc in playerCharacters)
        {
            var baseData = _cache.GetCharacter(pc.CharacterId);
            if (baseData == null)
                throw new RequestException(ErrorStatusCode.NotFound, "CHARACTER_NOT_FOUND",
                    new { CharacterId = pc.CharacterId });

            pc.Data = baseData;
        }

        return playerCharacters;
    }

    public async Task<PlayerCharacterData> AddPlayerCharacterDataByUserIdAsync(PlayerCharacterData data)
    {
        await using var db = CreateConnection();
        await db.OpenAsync();

        var sql = @"
            INSERT INTO paper_mania_game_data.player_character_data
            (user_id, character_id, character_level, normal_skill_level, epic_skill_level, character_pieces)
            VALUES (@UserId, @CharacterId, 1, 1, 1, 0);
        ";

        await db.ExecuteAsync(sql, new { UserId = data.UserId, CharacterId = data.CharacterId });

        var baseData = _cache.GetCharacter(data.CharacterId);
        if (baseData == null)
            throw new RequestException(ErrorStatusCode.NotFound, "CHARACTER_NOT_FOUND",
                new { CharacterId = data.CharacterId });

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
            UPDATE paper_mania_game_data.player_character_data
            SET character_pieces = character_pieces + @Amount
            WHERE user_id = @UserId AND character_id = @CharacterId
            RETURNING character_pieces;
        ";

        await db.ExecuteAsync(sql, new { Amount = amount, UserId = userId, CharacterId = characterId });
    }
}