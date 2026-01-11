using Server.Application.Port.Input.Reward;
using Server.Application.Port.Output.Persistence;
using Server.Application.UseCase.Reward.Command;

namespace Server.Application.UseCase.Reward;

public class CheckStageClearedUseCase : ICheckStageClearedUseCase
{
    private readonly IStageRepository _repository;

    public CheckStageClearedUseCase(IStageRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<bool> ExecuteAsync(CheckStageClearedCommand request)
    {
        request.Validate();
        
        var stageData  = await _repository.FindByUserIdAsync(
            request.UserId,
            request.StageNum,
            request.StageSubNum
            );

       return stageData != null;
    }
}