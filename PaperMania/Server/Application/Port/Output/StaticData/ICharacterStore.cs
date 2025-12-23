using Server.Infrastructure.StaticData.Model;

namespace Server.Application.Port.Output.StaticData;

public interface ICharacterStore
{
    CharacterData? Get(int characterId);
    IReadOnlyDictionary<int, CharacterData> GetAll();
}