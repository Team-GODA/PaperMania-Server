namespace Server.Application.Exceptions;

public class PlayerCharactersNotFoundException : Exception
{
    public PlayerCharactersNotFoundException(int? userId)
        : base($"{userId}에 대한 캐릭터 데이터를 찾을 수 없습니다.")
    {
    }
}