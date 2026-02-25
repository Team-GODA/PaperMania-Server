namespace Server.Infrastructure.Persistence.Model;

public record PlayerAccountData(
    int Id,
    string PlayerId,
    string Email,
    string Password,
    bool IsNewAccount,
    string Role,
    DateTime CreatedAt
    );