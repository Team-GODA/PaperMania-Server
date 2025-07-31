using Server.Api.Dto.Response;

namespace Server.Application.Exceptions.Data;

public class PlayerNameExistException : GameException
{
    public PlayerNameExistException(string playerName)
        : base(ErrorStatusCode.Conflict ,$"이미 존재하는 플레이어 이름입니다: {playerName}")
    {
    }
}