namespace Server.Domain.Entity;

public class PlayerCurrencyData
{
    public int UserId { get; set; }
    public int ActionPoint { get; set; }
    public int MaxActionPoint { get; set; }
    public int Gold { get; set; }
    public int PaperPiece { get; set; }
    public DateTime LastActionPointUpdated { get; set; }
}