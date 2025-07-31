namespace Server.Api.Dto.Response.Auth
{
    public class LoginResponse
    {
        public int Id { get; set; }
        public string SessionId { get; set; } = "";
    }
}