using Server.Application.Port.Input.Character;
using Server.Application.Port.Output.StaticData;
using Server.Infrastructure.StaticData.Model;

namespace Server.Application.UseCase.Character;

public class GetAllCharacterDataUseCase : IGetAllCharacterDataUseCase
{
    private readonly ICharacterStore _store;

    public GetAllCharacterDataUseCase(
        ICharacterStore store
        )
    {
        _store = store;
    }
    
    public List<CharacterData> Execute()
    {
        var data = _store.GetAll();
        
        return data.Values
            .OrderBy(c => c.CharacterId)
            .ToList();
    }
}