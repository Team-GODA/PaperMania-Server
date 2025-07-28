namespace Server.Application.Exceptions;

public class SessionNotFoundException : ApplicationException
{
    public SessionNotFoundException()
        : base("세션 ID가 없습니다.")
    {
    }
}