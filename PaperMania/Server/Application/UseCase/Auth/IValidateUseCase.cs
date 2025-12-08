using Server.Application.UseCase.Auth.Command;
using Server.Application.UseCase.Auth.Result;

namespace Server.Application.UseCase.Auth;

public interface IValidateUseCase
{
    public Task<ValidateResult> ExecuteAsync(ValidateCommand request);
}