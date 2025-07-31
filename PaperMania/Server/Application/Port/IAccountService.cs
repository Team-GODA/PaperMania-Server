using Server.Domain.Entity;

namespace Server.Application.Port;

public interface IAccountService
{
    Task<PlayerAccountData?> GetByPlayerIdAsync(string playerId);
    Task<PlayerAccountData?> GetByEmailAsync(string email);
    Task<PlayerAccountData?> RegisterAsync(PlayerAccountData player, string password);
    Task<(string sessionId, PlayerAccountData user)> LoginAsync(string playerId, string password);
    Task LogoutAsync(string sessionId);
    Task<string?> LoginByGoogleAsync(string idToken);
}