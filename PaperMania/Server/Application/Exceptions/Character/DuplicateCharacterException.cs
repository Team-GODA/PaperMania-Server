using Server.Api.Dto.Response;

namespace Server.Application.Exceptions.Character;

public class DuplicateCharacterException : GameException
{
    public DuplicateCharacterException(string chrarcterName)
        : base(ErrorStatusCode.Conflict, $"이미 해당 캐릭터를 보유 중입니다, 캐릭터 이름 : {chrarcterName}")
    {
    }
}