namespace Server.Infrastructure.Persistence.Model;

public record PlayerCurrencyData(
    int UserId, 
    int ActionPoint,
    int MaxActionPoint,
    int Gold,
    int PaperPiece,
    DateTime LastActionPointUpdated
    );