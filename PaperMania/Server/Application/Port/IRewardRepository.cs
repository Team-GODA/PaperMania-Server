using Server.Domain.Entity;

namespace Server.Application.Port;

public interface IRewardRepository
{
    StageReward? GetStageReward(int stageNum, int stageSubNum);
    Task ClaimStageRewardByUserIdAsync(int? userId, StageReward reward);
}