using Server.Api.Dto.Response;

namespace Server.Application.Exceptions.Data;

public class PlayerDataExistException : GameException
{
    public PlayerDataExistException()
        : base(ErrorStatusCode.Conflict ,"이미 이름을 등록한 계정입니다.")
    {
    }
}