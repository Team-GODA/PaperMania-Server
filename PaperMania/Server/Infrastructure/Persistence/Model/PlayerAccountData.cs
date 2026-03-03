namespace Server.Infrastructure.Persistence.Model;

public class PlayerAccountData
{
    public int Id { get; init; }
    public string PlayerId { get; init; } = "";
    public string Email { get; init; } = "";
    public string Password { get; init; } = "";
    public bool IsNewAccount { get; init; }
    public string Role { get; init; } = "";
    public DateTime CreatedAt { get; init; }
}