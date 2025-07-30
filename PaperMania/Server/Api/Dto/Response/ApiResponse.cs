namespace Server.Api.Dto.Response;

public static class ApiResponse
{
    public static BaseResponse<T> Ok<T>(string message, T data)
    {
        return new() { ErrorCode = 0, Message = message, Data = data };
    }
    
    public static BaseResponse<T?> Ok<T>(string message) where T : class
    {
        return new() { ErrorCode = 0, Message = message, Data = null };
    }

    public static BaseResponse<T> Error<T>(ErrorStatusCode code, string message)
    {
        return new() { ErrorCode = (int)code, Message = message, Data = default };
    }
}

public enum ErrorStatusCode
{
    Conflict = 1001,         // 이미 존재하거나 충돌
    Unauthorized = 1002,     // 인증 실패
    BadRequest = 1003,       // 잘못된 요청
    NotFound = 1004,         // 리소스 없음
    ServerError = 5000       // 서버 내부 오류
}