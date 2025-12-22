using Server.Application.UseCase.Reward.Command;
using Server.Infrastructure.StaticData;

namespace Server.Application.Port.Input.Reward;

public interface IGetStageRewardUseCase
{
    StageReward Execute(GetStageRewardCommand request);
}