namespace Server.Application.Exceptions;

public class PlayerNameExistException : Exception
{
    public PlayerNameExistException(string playerName)
        : base($"이미 존재하는 플레이어 이름입니다: {playerName}")
    {
    }
}