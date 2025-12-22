namespace Server.Infrastructure.Persistence.Model;

public class PlayerAccountData
{
    public int Id { get; set; }
    public string PlayerId { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public bool IsNewAccount { get; set; } = true;
    public string Role { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}