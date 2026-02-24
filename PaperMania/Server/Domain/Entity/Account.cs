using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Domain.Service;

namespace Server.Domain.Entity;

public class Account
{
    public int Id { get; set; }
    public string PlayerId  { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public bool IsNewAccount { get; set; }
    public string Role { get; set; } = "user";
    
    public Account(
        string playerId, 
        string email,
        string password, 
        bool isNewAccount)
    {
        PlayerId = playerId;
        Email = email;
        Password = password;
        IsNewAccount = isNewAccount;
    }
    
    public void VerifyPassword(string password, IPasswordHasher hasher)
    {
        if (!hasher.Verify(password, Password))
            throw new RequestException(
                ErrorStatusCode.Unauthorized,
                "INVALID_CREDENTIALS");
    }
}