using Server.Application.UseCase.Reward.Command;
using Server.Application.UseCase.Reward.Result;

namespace Server.Application.Port.Input.Reward;

public interface IClaimStageRewardUseCase
{
    Task<ClaimStageRewardResult> ExecuteAsync(ClaimStageRewardCommand request); 
}