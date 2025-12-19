using Server.Api.Dto.Response;
using Server.Application.Exceptions;

namespace Server.Application.UseCase.Currency.Command;

public record SpendGoldCommand(
    int UserId,
    int Gold
)
{
    public void Validate()
    {
        if (UserId <= 0)
            throw new RequestException(
                ErrorStatusCode.BadRequest,
                "INVALID_USER_ID");
        
        if (Gold < 0)
            throw new RequestException(
                ErrorStatusCode.BadRequest,
                "INVALID_GOLD_AMOUNT");
    }   
}