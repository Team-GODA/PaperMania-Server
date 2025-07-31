using Server.Api.Dto.Response;

namespace Server.Application.Exceptions.Auth;

public class DuplicateEmailException : GameException
{
    public DuplicateEmailException(string email)
        : base(ErrorStatusCode.Conflict ,$"이메일 '{email}'은 이미 사용 중입니다.")
    {
    }
}