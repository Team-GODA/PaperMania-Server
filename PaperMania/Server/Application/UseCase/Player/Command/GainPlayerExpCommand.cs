using Server.Api.Dto.Response;
using Server.Application.Exceptions;

namespace Server.Application.UseCase.Player.Command;

public record GainPlayerExpCommand(
    int UserId,
    int Exp
)
{
    public void Validate()
    {
        if (UserId <= 0)
            throw new RequestException(
                ErrorStatusCode.BadRequest,
                "INVALID_USER_ID");
        
        if (Exp < 0)
            throw new RequestException(
                ErrorStatusCode.BadRequest,
                "INVALID_Exp_AMOUNT");
    }
}