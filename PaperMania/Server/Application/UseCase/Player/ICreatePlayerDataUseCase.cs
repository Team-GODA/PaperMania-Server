using Server.Application.UseCase.Data.Command;
using Server.Application.UseCase.Data.Result;

namespace Server.Application.UseCase.Data;

public interface ICreatePlayerDataUseCase
{
    Task<AddPlayerDataResult> ExecuteAsync(AddPlayerDataCommand request);
}