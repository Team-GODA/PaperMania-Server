using Server.Api.Dto.Response;

namespace Server.Application.Exceptions;

public class RequestException : GameException
{
    public object?[] Context { get; }

    public RequestException(ErrorStatusCode statusCode, string message, params object?[] context)
        : base(statusCode, message)
    {
        Context = context;
    }
}