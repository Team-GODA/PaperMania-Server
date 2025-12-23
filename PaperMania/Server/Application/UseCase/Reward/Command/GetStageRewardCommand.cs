using Server.Api.Dto.Response;
using Server.Application.Exceptions;

namespace Server.Application.UseCase.Reward.Command;

public record GetStageRewardCommand(
    int StageNum,
    int StageSubNum
)
{
    public void Validate()
    {
        if (StageNum <= 0 ||
            StageSubNum <= 0)
            throw new RequestException(
                ErrorStatusCode.BadRequest,
                "INVALID_STAGE_NUM");
    }
}
    