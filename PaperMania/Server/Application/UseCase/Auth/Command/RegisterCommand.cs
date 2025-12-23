using Server.Api.Dto.Response;
using Server.Application.Exceptions;

namespace Server.Application.UseCase.Auth.Command;

public record RegisterCommand(
    string PlayerId,
    string Email,
    string Password
)
{
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(PlayerId))
            throw new RequestException(
                ErrorStatusCode.BadRequest,
                "NULL_PlayerId");
        
        if (string.IsNullOrWhiteSpace(Email))
            throw new RequestException(
                ErrorStatusCode.BadRequest,
                "NULL_Email");
        
        if (string.IsNullOrWhiteSpace(Password))
            throw new RequestException(
                ErrorStatusCode.BadRequest,
                "NULL_Password");
    }
}