namespace Server.Domain.Entity;

public class PlayerCurrencyData
{
    public int Id { get; set; }
    public int ActionPoint { get; set; } = 0;
    public int MaxActionPoint { get; set; } = 0;
    public int Gold { get; set; } = 0;
    public int PaperPiece { get; set; } = 0;
    public DateTime LastActionPointUpdated { get; set; }
}