using Server.Api.Dto.Response.Auth;
using Server.Application.UseCase.Auth.Command;
using Server.Application.UseCase.Auth.Result;

namespace Server.Application.UseCase.Auth;

public interface IRegisterUseCase
{
    Task<RegisterResult?> ExecuteAsync(RegisterCommand request);
}