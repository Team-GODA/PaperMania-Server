using Server.Application.UseCase.Player.Command;
using Server.Application.UseCase.Player.Result;

namespace Server.Application.Port.Input.Player;

public interface IGainPlayerExpUseCase
{
    Task<GainPlayerExpUseCaseResult> ExecuteAsync(GainPlayerExpCommand request);
}