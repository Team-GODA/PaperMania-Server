using Server.Application.UseCase.Player.Command;
using Server.Application.UseCase.Player.Result;

namespace Server.Application.Port.Input.Player;

public interface IGetPlayerLevelUseCase
{
    Task<GetPlayerLevelResult> ExecuteAsync(GetPlayerLevelCommand request);
}