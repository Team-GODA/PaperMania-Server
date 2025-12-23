namespace Server.Application.Port.Input.Auth;

public interface ILogoutUseCase
{
    Task ExecuteAsync(string sessionId);
}