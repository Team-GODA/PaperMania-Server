using Server.Infrastructure.StaticData;

namespace Server.Application.Port.Output.StaticData;

public interface IStageRewardStore
{
    StageReward? GetStageReward(int stageNum, int stageSubNum);
}