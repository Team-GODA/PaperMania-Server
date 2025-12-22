using Dapper;
using Server.Application.Port.Output.Infrastructure;
using Server.Application.Port.Output.Persistence;
using Server.Infrastructure.Persistence.Model;

namespace Server.Infrastructure.Persistence.Dao;

public class CharacterDao : DaoBase, ICharacterDao
{
    private static class Sql
    {
        public const string GetAllByUserId = @"
            SELECT PC.user_id AS UserId, PC.character_id AS CharacterId,
                   PC.character_level AS CharacterLevel,
                   PC.normal_skill_level AS NormalSkillLevel,
                   PC.ultimate_skill_level AS UltimateSkillLevel,
                   PC.support_skill_level AS SupportSkillLevel,
                   PP.character_piece AS PieceAmount
            FROM paper_mania_game_data.player_character_data PC
            LEFT JOIN paper_mania_game_data.player_character_piece_data PP
                ON PC.user_id = PP.user_id AND PC.character_id = PP.character_id
            WHERE PC.user_id = @UserId";

        public const string GetByUserId = @"
            SELECT user_id AS UserId,
                   character_id AS CharacterId,
                   character_level AS CharacterLevel,
                   normal_skill_level AS NormalSkillLevel,
                   ultimate_skill_level AS UltimateSkillLevel,
                   support_skill_level AS SupportSkillLevel
            FROM paper_mania_game_data.player_character_data
            WHERE user_id = @UserId AND character_id = @CharacterId
            ";

        public const string createData = @"
            INSERT INTO paper_mania_game_data.player_character_data (
                user_id, 
                character_id, 
                character_level, 
                character_exp, 
                normal_skill_level, 
                ultimate_skill_level, 
                support_skill_level
            )
            VALUES (    
                @UserId, 
                @CharacterId, 
                @CharacterLevel, 
                @CharacterExp, 
                @NormalSkillLevel, 
                @UltimateSkillLevel, 
                @SupportSkillLevel
            )
            RETURNING 
                user_id AS UserId,
                character_id AS CharacterId,
                character_level AS CharacterLevel,
                character_exp AS CharacterExp,
                normal_skill_level AS NormalSkillLevel,
                ultimate_skill_level AS UltimateSkillLevel,
                support_skill_level AS SupportSkillLevel
    ";
    }

    public CharacterDao(
        string connectionString,
        ITransactionScope? transactionScope = null
        ) : base(connectionString, transactionScope)
    {
    }

    public async Task<IEnumerable<PlayerCharacterData?>> FindAll(int userId)
    {
        return await QueryAsync(async conn =>
            (await conn.QueryAsync<PlayerCharacterData>(
                Sql.GetAllByUserId,
                new { UserId = userId }
                )
            ).ToList()
        );
    }

    public async Task<PlayerCharacterData?> FindCharacter(int userId, int characterId)
    {
        return await QueryAsync(async conn =>
            await conn.QuerySingleOrDefaultAsync<PlayerCharacterData>(
                Sql.GetByUserId,
                new
                {
                    UserId = userId,
                    CharacterId = characterId
                }
            )
        );
    }

    public async Task<PlayerCharacterData?> UpdateAsync(PlayerCharacterData data)
    {
        throw new NotImplementedException();
    }

    public async Task<PlayerCharacterData?> CreateAsync(PlayerCharacterData data)
    {
        return await ExecuteAsync((connection, transaction) =>
            connection.QuerySingleAsync<PlayerCharacterData>(
                Sql.createData, 
                data,
                transaction)
        );
    }
}