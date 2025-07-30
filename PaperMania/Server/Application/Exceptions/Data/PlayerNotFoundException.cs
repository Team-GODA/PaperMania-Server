namespace Server.Application.Exceptions.Data;

public class PlayerNotFoundException : Exception
{
    public PlayerNotFoundException(int? userId)
        : base($"{userId} 에 대한 플레이어 데이터를 찾을 수 없습니다.")
    {
    }
}