using Server.Api.Dto.Response;
using Server.Application.Exceptions;
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
            throw new RequestException(ErrorStatusCode.NotFound, "PLAYER_CHARACTERS_NOT_FOUND",
                new { UserId = userId });
            
        return data;
    }

    public async Task<PlayerCharacterData> AddPlayerCharacterDataByUserIdAsync(PlayerCharacterData data)
    {
        var exists = await _characterRepository.IsNewCharacterExistAsync(data.Id, data.CharacterId);
        if (exists)
            throw new RequestException(ErrorStatusCode.Conflict, "DUPLICATE_PLAYER_CHARACTER",
            new { CharacterId = data.CharacterId });
        
        return await _characterRepository.AddPlayerCharacterDataByUserIdAsync(data);
    }
}