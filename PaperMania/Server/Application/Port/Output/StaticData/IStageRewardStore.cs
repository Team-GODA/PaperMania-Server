using Server.Infrastructure.StaticData;
using Server.Infrastructure.StaticData.Model;

namespace Server.Application.Port.Output.StaticData;

public interface IStageRewardStore
{
    StageReward? GetStageReward(int stageNum, int stageSubNum);
}