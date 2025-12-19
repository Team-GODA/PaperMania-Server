using Server.Application.UseCase.Player.Command;
using Server.Application.UseCase.Player.Result;

namespace Server.Application.Port.Input.Player;

public interface ICreatePlayerDataUseCase
{
    Task<AddPlayerDataResult> ExecuteAsync(AddPlayerDataCommand request);
}