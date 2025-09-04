namespace Server.Api.Dto.Response.Auth
{
    public class LoginResponse
    {
        public string SessionId { get; set; } = "";
        public bool IsNewAccount { get; set; } = true;
    }
}