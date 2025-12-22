using Server.Application.Port.Input.Stage;
using Server.Application.Port.Output.Persistence;
using Server.Application.UseCase.Stage.Command;

namespace Server.Application.UseCase.Stage;

public class CheckStageClearedUseCase : ICheckStageClearedUseCase
{
    private readonly IStageDao _dao;

    public CheckStageClearedUseCase(IStageDao dao)
    {
        _dao = dao;
    }
    
    public async Task<bool> ExecuteAsync(CheckStageClearedCommand request)
    {
        request.Validate();
        
        var stageData  = _dao.FindByUserIdAsync(
            request.UserId,
            request.StageNum,
            request.StageSubNum
            );

       return await 
           stageData != null;
    }
}