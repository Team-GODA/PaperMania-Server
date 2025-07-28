namespace Server.Api.Dto.Response;

public class BaseResponse<T>
{
    public int ErrorCode { get; set; }
    public string Message { get; set; } = "성공적으로 처리되었습니다.";
    public T? Data { get; set; }
}