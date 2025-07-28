using Server.Api.Dto.Response;

namespace Server.Api;

public static class Response
{
    public static BaseResponse<T> Ok<T>(T data, string message)
    {
        return new() { ErrorCode = 0, Message = message, Data = data };
    }

    public static BaseResponse<T> Error<T>(int code, string message)
    {
        return new() { ErrorCode = code, Message = message, Data = default };
    }
}