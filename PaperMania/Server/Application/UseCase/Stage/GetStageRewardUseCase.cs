using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.Input.Stage;
using Server.Application.Port.Output.Persistence;
using Server.Application.Port.Output.StaticData;
using Server.Application.UseCase.Stage.Command;
using Server.Infrastructure.StaticData;

namespace Server.Application.UseCase.Stage;

public class GetStageRewardUseCase : IGetStageRewardUseCase
{
    private readonly IStageRewardStore _store;

    public GetStageRewardUseCase(IStageRewardStore store)
    {
        _store = store;
    }
    
    public StageReward Execute(GetStageRewardCommand request)
    {
        request.Validate();

        var stageData = _store.GetStageReward(request.StageNum, request.StageSubNum)
                        ?? throw new RequestException(
                            ErrorStatusCode.NotFound,
                            "STAGE_NOT_FOUND"
                            );
            
        return stageData;
    }
}