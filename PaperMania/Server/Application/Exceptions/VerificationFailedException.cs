namespace Server.Application.Exceptions;

public class VerificationFailedException : ApplicationException
{
    public VerificationFailedException(string message = "아이디 또는 비밀번호가 일치하지 않습니다.")
        : base(message)
    {
    }
}