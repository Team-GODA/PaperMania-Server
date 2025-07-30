namespace Server.Application.Exceptions.Auth;

public class GoogleLoginFailedException : Exception
{
    public GoogleLoginFailedException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}