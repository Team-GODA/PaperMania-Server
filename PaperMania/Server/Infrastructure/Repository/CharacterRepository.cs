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

    public async Task<IEnumerable<PlayerCharacterData>> GetPlayerCharacterDataByUserIdAsync(int? userId)
    {
        if (userId == null)
            return Enumerable.Empty<PlayerCharacterData>();

        await using var db = CreateConnection();
        await db.OpenAsync();

        var sql = @"
            SELECT user_id AS UserId, character_id AS CharacterId,
                   character_level AS CharacterLevel,
                   normal_skill_level AS NormalSkillLevel,
                   epic_skill_level AS EpicSkillLevel,
                   character_pieces AS CharacterPieces
            FROM paper_mania_game_data.player_character_data
            WHERE user_id = @UserId
        ";

        var playerCharacters = (await db.QueryAsync<PlayerCharacterData>(sql, new { Id = userId })).ToList();

        foreach (var pc in playerCharacters)
        {
            var baseData = _cache.GetCharacter(pc.CharacterId);
            if (baseData == null)
                throw new RequestException(ErrorStatusCode.NotFound, "CHARACTER_NOT_FOUND",
                    new { CharacterId = pc.CharacterId });

            pc.CharacterName = baseData.CharacterName;
            pc.Rarity = baseData.Rarity;
        }

        return playerCharacters;
    }

    public async Task<PlayerCharacterData> AddPlayerCharacterDataByUserIdAsync(PlayerCharacterData data)
    {
        await using var db = CreateConnection();
        await db.OpenAsync();

        var sql = @"
            INSERT INTO paper_mania_game_data.player_character_data (user_id, character_id)
            VALUES (@UserId, @CharacterId);
        ";

        var param = new { UserId = data.UserId, CharacterId = data.CharacterId };
        await db.ExecuteAsync(sql, param);

        var baseData = _cache.GetCharacter(data.CharacterId);
        if (baseData == null)
            throw new RequestException(ErrorStatusCode.NotFound, "CHARACTER_NOT_FOUND",
                new { CharacterId = data.CharacterId });

        data.CharacterName = baseData.CharacterName;
        data.Rarity = baseData.Rarity;

        return data;
    }

    public async Task<bool> IsNewCharacterExistAsync(int userId, string characterId)
    {
        await using var db = CreateConnection();
        await db.OpenAsync();

        var sql = @"
            SELECT 1
            FROM paper_mania_game_data.player_character_data
            WHERE user_id = @UserId AND character_id = @CharacterId
            LIMIT 1;
        ";

        var result = await db.QueryFirstOrDefaultAsync<int?>(sql, new { UserId = userId, CharacterId = characterId });
        return result.HasValue;
    }

    public async Task AddCharacterPiecesAsync(int userId, string characterId, int amount)
    {
        await using var db = CreateConnection();
        await db.OpenAsync();

        var sql = @"
            INSERT INTO paper_mania_game_data.player_character_piece (user_id, character_id, pieces)
            VALUES (@UserId, @CharacterId, @Amount)
            ON CONFLICT (user_id, character_id)
            DO UPDATE SET pieces = player_character_piece.pieces + @Amount;
        ";

        await db.ExecuteAsync(sql, new { Amount = amount, UserId = userId, CharacterId = characterId });
    }
}
