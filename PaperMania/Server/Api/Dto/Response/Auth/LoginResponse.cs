namespace Server.Api.Dto.Response.Auth
{
    public class LoginResponse
    {
        public string Message { get; set; } = "";
        public int Id { get; set; }
        public string SessionId { get; set; } = "";
    }
}