using Server.Api.Dto.Response;
using Server.Application.Exceptions;

namespace Server.Application.UseCase.Currency.Command;

public record GetCurrencyDataCommand(
    int UserId)
{
    public void Validate()
    {
        if (UserId <= 0)
            throw new RequestException(
                ErrorStatusCode.BadRequest,
                "INVALID_USER_ID");
    }
}