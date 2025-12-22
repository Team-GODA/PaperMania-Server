using Server.Application.UseCase.Character.Command;
using Server.Infrastructure.Persistence.Model;

namespace Server.Application.Port.Input.Character;

public interface IGetCharacterUseCase
{
    Task<PlayerCharacterData> ExecuteAsync(GetCharacterCommand request);
}