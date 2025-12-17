using Server.Application.UseCase.Auth.Command;
using Server.Application.UseCase.Auth.Result;

namespace Server.Application.Port.In.Auth;

public interface ILoginUseCase
{
    Task<LoginResult> ExecuteAsync(LoginCommand request);
}