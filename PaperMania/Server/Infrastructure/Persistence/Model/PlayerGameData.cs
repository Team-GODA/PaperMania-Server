namespace Server.Infrastructure.Persistence.Model;

public class PlayerGameData
{
    public int UserId { get; set; }
    public string PlayerName { get; set; } = null!;
    public int PlayerExp { get; set; }
    public int PlayerLevel { get; set; } = 1;
}