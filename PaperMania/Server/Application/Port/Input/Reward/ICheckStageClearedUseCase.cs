using Server.Application.UseCase.Reward.Command;

namespace Server.Application.Port.Input.Reward;

public interface ICheckStageClearedUseCase
{
    Task<bool> ExecuteAsync(CheckStageClearedCommand request);
}