using Server.Application.UseCase.Character.Command;
using Server.Infrastructure.Persistence.Model;

namespace Server.Application.Port.Input.Character;

public interface IGetPlayerCharacterUseCase
{
    Task<PlayerCharacterData> ExecuteAsync(GetPlayerCharacterCommand request);
}