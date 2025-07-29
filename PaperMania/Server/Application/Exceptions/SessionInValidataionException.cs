namespace Server.Application.Exceptions;

public class SessionInValidataionException : ApplicationException
{
    public SessionInValidataionException(string message = "유효하지 않는 SID 입니다")
        : base(message)
    {
    }
}