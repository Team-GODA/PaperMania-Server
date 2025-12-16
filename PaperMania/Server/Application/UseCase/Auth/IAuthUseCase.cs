using Server.Api.Dto.Request;
using Server.Application.UseCase.Auth.Command;
using Server.Application.UseCase.Auth.Result;

namespace Server.Application.UseCase.Auth;

public interface IAuthUseCase
{
    Task<LoginResult> LoginAsync(LoginCommand request);
    Task RegisterAsync(RegisterCommand request);
    Task ValidateAsync(string sessionId);
    Task LogoutAsync(string sessionId);
}