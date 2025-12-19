using Server.Application.UseCase.Auth.Command;

namespace Server.Application.Port.Input.Auth;

public interface IRegisterUseCase
{
    Task ExecuteAsync(RegisterCommand request);
}