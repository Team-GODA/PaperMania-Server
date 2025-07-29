namespace Server.Application.Exceptions;

public class GoogleLoginFailedException : ApplicationException
{
    public GoogleLoginFailedException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}