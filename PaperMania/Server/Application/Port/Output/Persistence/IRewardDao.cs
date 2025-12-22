using Server.Domain.Entity;
using Server.Infrastructure.StaticData;

namespace Server.Application.Port.Output.Persistence;

public interface IRewardDao
{
    StageReward? GetStageReward(int stageNum, int stageSubNum);
    Task ClaimStageRewardByUserIdAsync(int userId, StageReward reward);
}