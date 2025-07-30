namespace Server.Application.Exceptions;

public class DuplicateEmailException : Exception
{
    public DuplicateEmailException(string email)
        : base($"이메일 '{email}'은 이미 사용 중입니다.")
    {
    }
}