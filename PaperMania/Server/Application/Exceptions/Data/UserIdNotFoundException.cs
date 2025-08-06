using Server.Api.Dto.Response;

namespace Server.Application.Exceptions.Data;

public class UserIdNotFoundException :  GameException
{
    public UserIdNotFoundException(int? userId)
        : base(ErrorStatusCode.NotFound, $"Id : {userId}를 찾을 수 없습니다")
    {
    }
}