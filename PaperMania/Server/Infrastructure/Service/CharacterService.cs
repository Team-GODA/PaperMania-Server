using Server.Application.Exceptions;
using Server.Application.Exceptions.Character;
using Server.Application.Port;
using Server.Domain.Entity;

namespace Server.Infrastructure.Service;

public class CharacterService : ICharacterService
{
    private readonly ICharacterRepository _characterRepository;

    public CharacterService(ICharacterRepository characterRepository)
    {
        _characterRepository = characterRepository;
    }
    
    public async Task<IEnumerable<PlayerCharacterData>> GetPlayerCharacterDataByUserIdAsync(int? userId)
    {
        var data = (await _characterRepository.GetPlayerCharacterDataByUserIdAsync(userId)).ToList();
        if (data.Count == 0)
            throw new PlayerCharactersNotFoundException(userId);
            
        return data;
    }

    public async Task<PlayerCharacterData> AddPlayerCharacterDataByUserIdAsync(PlayerCharacterData data)
    {
        bool exists = await _characterRepository.IsNewCharacterExistAsync(data.Id, data.CharacterId);
        if (exists)
            throw new InvalidOperationException("이미 해당 캐릭터를 보유 중입니다.");
        
        return await _characterRepository.AddPlayerCharacterDataByUserIdAsync(data);
    }
}