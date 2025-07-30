namespace Server.Application.Exceptions.Data;

public class PlayerDataExistException : Exception
{
    public PlayerDataExistException()
        : base("이미 이름을 등록한 계정입니다.")
    {
    }
}