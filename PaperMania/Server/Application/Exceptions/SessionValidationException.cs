namespace Server.Application.Exceptions;

public class SessionValidationException : ApplicationException
{
    public SessionValidationException(string message = "유효하지 않는 SID 입니다")
        : base(message)
    {
    }
}