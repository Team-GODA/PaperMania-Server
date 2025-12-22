using Server.Domain.Entity;
using Server.Infrastructure.Persistence.Model;
using Server.Infrastructure.StaticData;

namespace Server.Application.Port.Output.Service;

public interface IRewardService
{
    StageReward? GetStageReward(int stageNum, int stageSubNum);
    Task ClaimStageRewardByUserIdAsync(int userId, StageReward reward, PlayerStageData data);
}