using Server.Application.UseCase.Auth.Command;

namespace Server.Application.UseCase.Auth;

public interface ILogoutUseCase
{
    Task ExecuteAsync(LogoutCommand request);
}