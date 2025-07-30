namespace Server.Application.Exceptions.Auth;

public class AuthenticationFailedException : Exception
{
    public AuthenticationFailedException(string message)
        : base(message)
    {
    }
}