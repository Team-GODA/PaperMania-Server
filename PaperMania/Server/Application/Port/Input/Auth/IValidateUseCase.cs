namespace Server.Application.Port.Input.Auth;

public interface IValidateUseCase
{
    Task ExecuteAsync(string sessionId);
}