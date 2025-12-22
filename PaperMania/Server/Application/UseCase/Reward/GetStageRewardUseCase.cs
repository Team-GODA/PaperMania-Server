using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.Input.Reward;
using Server.Application.Port.Output.StaticData;
using Server.Application.UseCase.Reward.Command;
using Server.Infrastructure.StaticData;
using Server.Infrastructure.StaticData.Model;

namespace Server.Application.UseCase.Reward;

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

        var stageReward = _store.GetStageReward(request.StageNum, request.StageSubNum)
                          ?? throw new RequestException(
                              ErrorStatusCode.NotFound,
                              "STAGE_NOT_FOUND"
                          );
            
        return stageReward;
    }
}