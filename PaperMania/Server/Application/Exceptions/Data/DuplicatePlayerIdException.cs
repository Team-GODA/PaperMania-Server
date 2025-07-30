namespace Server.Application.Exceptions.Data;

public class DuplicatePlayerIdException : Exception
{
    public DuplicatePlayerIdException(string playerId)
        : base($"플레이어 ID '{playerId}'은 이미 사용 중입니다.")
    {
    }
}