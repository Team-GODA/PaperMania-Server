using Server.Application.UseCase.Stage.Command;
using Server.Infrastructure.StaticData;

namespace Server.Application.Port.Input.Stage;

public interface IGetStageRewardUseCase
{
    StageReward Execute(GetStageRewardCommand request);
}