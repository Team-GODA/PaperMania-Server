using Server.Application.UseCase.Data.Command;

namespace Server.Application.UseCase.Data;

public interface IAddPlayerDataUseCase
{
    Task ExecuteAsync(AddPlayerDataCommand request);
}