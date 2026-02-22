using Dapper;
using Server.Application.Port.Output.Persistence;
using Server.Application.Port.Output.Transaction;
using Server.Infrastructure.Persistence.Model;

namespace Server.Infrastructure.Persistence.Repository;

public class CharacterRepository : RepositoryBase, ICharacterRepository
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
            WHERE PC.user_id = @UserId
            ";    

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
            ";

        public const string CreatePieceData = @"
            INSERT INTO paper_mania_game_data.player_character_piece_data (
                user_id,
                character_id,
                character_piece
            )
            VALUES (
                @UserId,
                @CharacterId,
                @PieceAmount
            )
            ";
            
        public const string UpdateData = @"
            UPDATE paper_mania_game_data.player_character_data
            SET
                character_level = @CharacterLevel,
                character_exp = @CharacterExp,
                normal_skill_level = @NormalSkillLevel,
                ultimate_skill_level = @UltimateSkillLevel,
                support_skill_level = @SupportSkillLevel
            WHERE user_id = @UserId
                AND character_id = @CharacterId
            ";
    }

    public CharacterRepository(
        string connectionString,
        ITransactionScope? transactionScope = null
        ) : base(connectionString, transactionScope)
    {
    }

    public async Task<IEnumerable<PlayerCharacterData>> FindAll(int userId, CancellationToken ct)
    {
        return await QueryAsync(conn =>
            conn.QueryAsync<PlayerCharacterData>(
                new CommandDefinition(Sql.GetAllByUserId, new { UserId = userId }, cancellationToken: ct)
            ), ct);
    }

    public async Task<PlayerCharacterData?> FindCharacter(int userId, int characterId, CancellationToken ct)
    {
        return await QueryAsync(conn =>
            conn.QuerySingleOrDefaultAsync<PlayerCharacterData>(
                new CommandDefinition(Sql.GetByUserId, new { UserId = userId, CharacterId = characterId }, cancellationToken: ct)
            ), ct);
    }

    public async Task<PlayerCharacterData> UpdateAsync(PlayerCharacterData data, CancellationToken ct)
    {
        await ExecuteAsync((connection, transaction) =>
            connection.ExecuteAsync(
                new CommandDefinition(Sql.UpdateData, data, transaction: transaction, cancellationToken: ct)
            ), ct);

        return data;
    }

    public async Task CreateAsync(PlayerCharacterData data, CancellationToken ct)
    {
        await ExecuteAsync((connection, transaction) =>
            connection.ExecuteAsync(
                new CommandDefinition(Sql.createData, data, transaction: transaction, cancellationToken: ct)
            ), ct);
    }

    public async Task CreatePieceData(PlayerCharacterPieceData data, CancellationToken ct)
    {
        await ExecuteAsync((connection, transaction) =>
            connection.ExecuteAsync(
                new CommandDefinition(Sql.CreatePieceData, data, transaction: transaction, cancellationToken: ct)
            ), ct);
    }
}