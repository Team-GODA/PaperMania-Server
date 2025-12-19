using Server.Application.UseCase.Player.Command;
using Server.Application.UseCase.Player.Result;

namespace Server.Application.Port.In.Player;

public interface IRenameUseCase
{
    Task<RenameResult> ExecuteAsync(RenameCommand request);
}