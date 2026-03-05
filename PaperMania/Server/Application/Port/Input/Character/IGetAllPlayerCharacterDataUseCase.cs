using Server.Domain.Entity;

namespace Server.Application.Port.Input.Character;

public interface IGetAllPlayerCharacterDataUseCase
{
    Task<List<PlayerCharacter>> ExecuteAsync(int userId, CancellationToken ct);
}