namespace Server.Api.Dto.Response.Auth;

public class ValidateUserResponse
{
    public int UserId { get; set; }
    public bool IsValidated { get; set; }
}