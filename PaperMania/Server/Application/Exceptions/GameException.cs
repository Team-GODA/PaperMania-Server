using Server.Api.Dto.Response;

namespace Server.Application.Exceptions;

public class GameException : Exception
{
    public ErrorStatusCode StatusCode { get; }

    protected GameException(ErrorStatusCode statusCode, string message) : base(message)
    {
        StatusCode = statusCode;
    }
}