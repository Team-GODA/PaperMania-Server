using Server.Application.UseCase.Character.Command;
using Server.Domain.Entity;

namespace Server.Application.Port.Input.Character;

public interface IGetPlayerCharacterUseCase
{
    Task<PlayerCharacter> ExecuteAsync(GetPlayerCharacterCommand request, CancellationToken ct);
}