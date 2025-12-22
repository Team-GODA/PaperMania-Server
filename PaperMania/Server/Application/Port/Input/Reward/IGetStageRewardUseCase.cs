using Server.Application.UseCase.Reward.Command;
using Server.Infrastructure.StaticData;
using Server.Infrastructure.StaticData.Model;

namespace Server.Application.Port.Input.Reward;

public interface IGetStageRewardUseCase
{
    StageReward Execute(GetStageRewardCommand request);
}