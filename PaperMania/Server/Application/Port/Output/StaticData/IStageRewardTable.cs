using Server.Domain.Entity;

namespace Server.Application.Port.Output.StaticData;

public interface IStageRewardTable
{
    StageReward Get(int stageNum, int stageSubNum);
}