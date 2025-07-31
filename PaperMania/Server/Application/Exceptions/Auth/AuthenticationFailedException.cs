using Server.Api.Dto.Response;

namespace Server.Application.Exceptions.Auth;

public class AuthenticationFailedException : GameException
{
    public AuthenticationFailedException(string message)
        : base(ErrorStatusCode.Unauthorized, message)
    {
    }
}