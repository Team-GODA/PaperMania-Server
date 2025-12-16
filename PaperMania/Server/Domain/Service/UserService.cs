namespace Server.Domain.Service;

public class UserService
{
    public bool VerifyPassword(string password, string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(password) || 
            string.IsNullOrWhiteSpace(hashedPassword))
            return false;
        
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
    
    public string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("EMPTY_PASSWORD", nameof(password));
        
        return BCrypt.Net.BCrypt.HashPassword(password, 10);
    }
}