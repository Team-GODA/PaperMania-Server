namespace Server.Infrastructure.Persistence.Model;

public record PlayerCharacterPieceData(
    int UserId, 
    int CharacterId, 
    int PieceAmount
);