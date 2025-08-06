using Dapper;
using Server.Application.Port;
using Server.Domain.Entity;

namespace Server.Infrastructure.Repository;

public class CharacterRepository : RepositoryBase, ICharacterRepository
{
    public CharacterRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<IEnumerable<PlayerCharacterData>> GetPlayerCharacterDataByUserIdAsync(int? userId)
    {
        await using var db = CreateConnection();
        await db.OpenAsync();
        
        var sql = @"
            SELECT P.user_id AS Id, P.character_id AS CharacterId, P.character_level AS CharacterLevel,
                   P.normal_skill_level AS NormalSkillLevel, P.epic_skill_level AS EpicSkillLevel,
                   C.character_name AS CharacterName, C.rarity AS RarityString
            FROM paper_mania_game_data.player_character_data P
            JOIN paper_mania_game_data.character_data C ON P.character_id = C.character_id
            WHERE P.user_id = @Id
            ";

        var result = (await db.QueryAsync<PlayerCharacterData>(sql, new { Id = userId })).ToList();
        return result;
    }

    public async Task<PlayerCharacterData> AddPlayerCharacterDataByUserIdAsync(PlayerCharacterData data)
    {
        await using var db = CreateConnection();
        await db.OpenAsync();
        
        var sql = @"
            INSERT INTO paper_mania_game_data.player_character_data (user_id, character_id)
            VALUES (@UserId, @CharacterId);
            ";
        
        var param = new
        {
            UserId = data.Id,
            CharacterId  = data.CharacterId
        };
        
        await db.ExecuteAsync(sql, param);

        var result = await db.QuerySingleAsync<PlayerCharacterData>(@"
                SELECT 
                    P.user_id AS Id,
                    P.character_id AS CharacterId,
                    P.character_level AS CharacterLevel,
                    P.normal_skill_level AS NormalSkillLevel,
                    P.epic_skill_level AS EpicSkillLevel,
                    C.character_name AS CharacterName,
                    C.rarity AS RarityString
                FROM paper_mania_game_data.player_character_data P
                JOIN paper_mania_game_data.character_data C ON P.character_id = C.character_id
                WHERE P.user_id = @UserId AND P.character_id = @CharacterId",
            new { UserId =data.Id, data.CharacterId });
        
        return result;
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

    public async Task<PlayerCharacterData?> GetCharacterByUserIdAsync(int userId)
    {
        await using var db = CreateConnection();
        await db.OpenAsync();
        
        var sql = @"
            SELECT user_id AS Id, character_name AS CharacterName, rarity AS RarityString
            FROM paper_mania_game_data.player_character_data
            WHERE user_id = @Id
            ";

        return await db.QuerySingleOrDefaultAsync<PlayerCharacterData>(sql, new { Id = userId });
    }
}