namespace Server.Application.Exceptions.Data;

public class PlayerNameMissingException : Exception
{
    public PlayerNameMissingException()
        : base("플레이어 이름 재설정 실패: NewName 누락 오류")
    {
    }
}