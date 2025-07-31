using Server.Api.Dto.Response;

namespace Server.Application.Exceptions.Data;

public class PlayerNameMissingException : GameException
{
    public PlayerNameMissingException()
        : base(ErrorStatusCode.BadRequest ,"플레이어 이름 재설정 실패: NewName 누락 오류")
    {
    }
}