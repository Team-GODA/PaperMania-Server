using Server.Application.UseCase.Auth.Command;

namespace Server.Application.Port.In.Auth;

public interface IRegisterUseCase
{
    Task ExecuteAsync(RegisterCommand request);
}