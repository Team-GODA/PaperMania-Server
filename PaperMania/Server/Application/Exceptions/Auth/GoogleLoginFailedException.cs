using Server.Api.Dto.Response;

namespace Server.Application.Exceptions.Auth;

public class GoogleLoginFailedException : GameException
{
    public GoogleLoginFailedException(string message)
        : base(ErrorStatusCode.Unauthorized ,message)
    {
    }
}