using Server.Domain.Entity;
using Server.Infrastructure.StaticData;

namespace Server.Application.Port.Output.StaticData;

public interface IStageRewardTable
{
    StageReward Get(int stageNum, int stageSubNum);
}