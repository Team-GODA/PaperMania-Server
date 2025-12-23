using Server.Infrastructure.Persistence.Model;

namespace Server.Application.Port.Input.Character;

public interface IGetAllPlayerCharacterDataUseCase
{
    Task<List<PlayerCharacterData>> ExecuteAsync(int userId);
}