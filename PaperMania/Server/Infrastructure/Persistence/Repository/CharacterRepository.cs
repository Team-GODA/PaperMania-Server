using Dapper;
using Server.Application.Port.Output.Persistence;
using Server.Application.Port.Output.Transaction;
using Server.Domain.Entity;
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
    
    private static PlayerCharacter MapToEntity(PlayerCharacterData data)
    {
        return new PlayerCharacter(
            data.UserId,
            data.CharacterId,
            data.CharacterLevel,
            data.CharacterExp,
            data.NormalSkillLevel,
            data.UltimateSkillLevel,
            data.SupportSkillLevel,
            data.PieceAmount
        );
    }
    
    private static PlayerCharacterData MapToData(PlayerCharacter entity)
    {
        return new PlayerCharacterData
        {
            UserId = entity.UserId,
            CharacterId = entity.CharacterId,
            CharacterLevel = entity.CharacterLevel,
            CharacterExp = entity.CharacterExp,
            NormalSkillLevel = entity.NormalSkillLevel,
            UltimateSkillLevel = entity.UltimateSkillLevel,
            SupportSkillLevel = entity.SupportSkillLevel,
            PieceAmount = entity.PieceAmount
        };
    }
    
    private static PlayerCharacterPieceData MapToPieceData(PlayerCharacter entity)
    {
        return new PlayerCharacterPieceData
        {
            UserId = entity.UserId,
            CharacterId = entity.CharacterId,
            PieceAmount = entity.PieceAmount
        };
    }

    public async Task<IEnumerable<PlayerCharacter>> FindAll(int userId, CancellationToken ct)
    {
        var rows = await QueryAsync(conn =>
            conn.QueryAsync<PlayerCharacterData>(
                new CommandDefinition(Sql.GetAllByUserId, new { UserId = userId }, cancellationToken: ct)
            ), ct);

        return rows.Select(MapToEntity);
    }

    public async Task<PlayerCharacter?> FindCharacter(int userId, int characterId, CancellationToken ct)
    {
        var row = await QueryAsync(conn =>
            conn.QuerySingleOrDefaultAsync<PlayerCharacterData>(
                new CommandDefinition(Sql.GetByUserId, new { UserId = userId, CharacterId = characterId }, cancellationToken: ct)
            ), ct);

        return MapToEntity(row);
    }

    public async Task<PlayerCharacter> UpdateAsync(PlayerCharacter entity, CancellationToken ct)
    {
        var data = MapToData(entity);

        await ExecuteAsync((connection, transaction) =>
            connection.ExecuteAsync(
                new CommandDefinition(Sql.UpdateData, data, transaction: transaction, cancellationToken: ct)
            ), ct);

        return entity;
    }

    public async Task CreateAsync(PlayerCharacter entity, CancellationToken ct)
    {
        var data = MapToData(entity);

        await ExecuteAsync((connection, transaction) =>
            connection.ExecuteAsync(
                new CommandDefinition(Sql.createData, data, transaction: transaction, cancellationToken: ct)
            ), ct);
    }

    public async Task CreatePieceData(PlayerCharacter entity, CancellationToken ct)
    {
        var pieceData = MapToPieceData(entity);

        await ExecuteAsync((connection, transaction) =>
            connection.ExecuteAsync(
                new CommandDefinition(Sql.CreatePieceData, pieceData, transaction: transaction, cancellationToken: ct)
            ), ct);
    }
}