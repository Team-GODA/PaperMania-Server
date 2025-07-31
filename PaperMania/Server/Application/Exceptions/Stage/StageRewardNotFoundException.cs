using Server.Api.Dto.Response;

namespace Server.Application.Exceptions.Stage;

public class StageRewardNotFoundException : GameException
{
    public StageRewardNotFoundException(int stageNum, int stageSubNum)
        : base(ErrorStatusCode.NotFound ,$"스테이지 {stageNum} - {stageSubNum} 에 대한 보상이 존재하지 않습니다.")
    {
    }
}