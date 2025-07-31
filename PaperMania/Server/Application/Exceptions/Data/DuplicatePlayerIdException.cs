using Server.Api.Dto.Response;

namespace Server.Application.Exceptions.Data;

public class DuplicatePlayerIdException : GameException
{
    public DuplicatePlayerIdException(string playerId)
        : base(ErrorStatusCode.Conflict ,$"플레이어 ID '{playerId}'은 이미 사용 중입니다.")
    {
    }
}