namespace Server.Api.Dto.Response;

public class BaseResponse<T>
{
    public int ErrorCode { get; set; }
    public string Message { get; set; }
    public T? Data { get; set; }
}