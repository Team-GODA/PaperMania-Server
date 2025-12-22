using Server.Application.UseCase.Stage.Command;

namespace Server.Application.Port.Input.Stage;

public interface ICheckStageClearedUseCase
{
    Task<bool> ExecuteAsync(CheckStageClearedCommand request);
}