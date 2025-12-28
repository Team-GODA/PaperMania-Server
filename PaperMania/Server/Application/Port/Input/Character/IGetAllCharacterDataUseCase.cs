using Server.Infrastructure.StaticData.Model;

namespace Server.Application.Port.Input.Character;

public interface IGetAllCharacterDataUseCase
{
    List<CharacterData> Execute();
}