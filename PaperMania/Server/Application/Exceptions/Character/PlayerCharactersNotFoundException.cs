using Server.Api.Dto.Response;

namespace Server.Application.Exceptions.Character;

public class PlayerCharactersNotFoundException : GameException
{
    public PlayerCharactersNotFoundException(int? userId)
        : base(ErrorStatusCode.NotFound ,$"{userId}에 대한 캐릭터 데이터를 찾을 수 없습니다.")
    {
    }
}