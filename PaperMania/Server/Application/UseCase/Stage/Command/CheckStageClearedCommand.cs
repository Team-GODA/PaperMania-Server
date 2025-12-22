using Server.Api.Dto.Response;
using Server.Application.Exceptions;

namespace Server.Application.UseCase.Stage.Command;

public record CheckStageClearedCommand(
    int UserId,
    int StageNum,
    int StageSubNum
)
{
    public void Validate()
    {
        if (UserId <= 0)
            throw new RequestException(
                ErrorStatusCode.BadRequest,
                "INVALID_USER_ID");

        if (StageNum <= 0 ||
            StageSubNum <= 0)
            throw new RequestException(
                ErrorStatusCode.BadRequest,
                "INVALID_STAGE_NUM");
    }
}