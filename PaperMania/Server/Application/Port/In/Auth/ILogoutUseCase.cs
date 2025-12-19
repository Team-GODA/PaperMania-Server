namespace Server.Application.Port.In.Auth;

public interface ILogoutUseCase
{
    Task ExecuteAsync(string sessionId);
}