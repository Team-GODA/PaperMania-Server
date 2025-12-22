using Server.Api.Dto.Response;
using Server.Application.Exceptions;

namespace Server.Application.UseCase.Character.Command;

public record CreatePlayerCharacterCommand(
    int UserId,
    int CharacterId
)
{
    public void Validate()
    {
        if (UserId <= 0)
            throw new RequestException(
                ErrorStatusCode.BadRequest,
                "INVALID_USER_ID");
        
        if (CharacterId <= 0)
            throw new RequestException(
                ErrorStatusCode.BadRequest,
                "INVALID_CHARACTER_ID");
    }
}