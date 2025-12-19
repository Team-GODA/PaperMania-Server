using Server.Application.UseCase.Player.Command;
using Server.Application.UseCase.Player.Result;

namespace Server.Application.Port.In.Player;

public interface IGetPlayerNameUseCase
{
    Task<GetPlayerNameResult> ExecuteAsync(GetPlayerNameCommand request);
}