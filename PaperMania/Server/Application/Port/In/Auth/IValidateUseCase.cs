namespace Server.Application.Port.In.Auth;

public interface IValidateUseCase
{
    Task ExecuteAsync(string sessionId);
}