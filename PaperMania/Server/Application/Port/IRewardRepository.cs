using Server.Domain.Entity;

namespace Server.Application.Port;

public interface IRewardRepository
{
    Task<StageReward?> GetStageRewardAsync(int stageNum, int stageSubNum);
    Task ClaimStageRewardByUserIdAsync(int? userId, StageReward reward);
}