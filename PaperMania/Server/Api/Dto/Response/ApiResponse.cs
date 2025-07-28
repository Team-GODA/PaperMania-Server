namespace Server.Api.Dto.Response;

public static class ApiResponse
{
    public static BaseResponse<T> Ok<T>(string message, T data)
    {
        return new() { ErrorCode = 0, Message = message, Data = data };
    }

    public static BaseResponse<T> Error<T>(int code, string message)
    {
        return new() { ErrorCode = code, Message = message, Data = default };
    }
}