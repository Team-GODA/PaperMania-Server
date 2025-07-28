namespace Server.Application.Exceptions;

public class DuplicateEmailException : ApplicationException
{
    public DuplicateEmailException(string email)
        : base($"이메일 '{email}'은 이미 사용 중입니다.")
    {
    }
}