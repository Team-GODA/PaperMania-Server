namespace Server.Application.Exceptions;

public class StageRewardNotFoundException : Exception
{
    public StageRewardNotFoundException(int stageNum, int stageSubNum)
        : base($"스테이지 {stageNum} - {stageSubNum} 에 대한 보상이 존재하지 않습니다.")
    {
    }
}