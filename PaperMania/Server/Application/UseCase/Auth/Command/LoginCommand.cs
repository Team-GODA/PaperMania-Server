using Server.Api.Dto.Response;
using Server.Application.Exceptions;

namespace Server.Application.UseCase.Auth.Command;

public record LoginCommand(
    string PlayerId,
    string Password
)
{
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(PlayerId))
            throw new RequestException(
                ErrorStatusCode.BadRequest,
                "NULL_PlayerId");
    }
}