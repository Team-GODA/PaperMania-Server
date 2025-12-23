using Server.Api.Dto.Response;
using Server.Application.Exceptions;

namespace Server.Application.UseCase.Player.Command;

public record RenameCommand(
    int? UserId,
    string NewName)
{
    public void Validate()
    {
        if (string.IsNullOrEmpty(NewName))
            throw new RequestException(
                ErrorStatusCode.BadRequest,
                "NEW_NAME_EMPTY");
    }
}