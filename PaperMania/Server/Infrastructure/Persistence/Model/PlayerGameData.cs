namespace Server.Infrastructure.Persistence.Model;

public class PlayerGameData
{
    public int UserId { get; set; }
    public string Name { get; set; } = null!;
    public int Exp { get; set; }
    public int Level { get; set; } = 1;
}