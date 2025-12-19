using Server.Application.UseCase.Player.Command;
using Server.Application.UseCase.Player.Result;

namespace Server.Application.Port.In.Player;

public interface IGainPlayerExpUseCase
{
    Task<GainPlayerExpUseCaseResult> ExecuteAsync(GainPlayerExpUseCaseCommand request);
}