namespace Server.Application.Exceptions;

public class GoogleLoginFailedException : Exception
{
    public GoogleLoginFailedException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}