using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port;
using Server.Domain.Entity;

namespace Server.Infrastructure.Service;

public class CharacterService : ICharacterService
{
    private readonly CharacterDataCache _cache;
    private readonly ICharacterRepository _repository;

    public CharacterService(CharacterDataCache cache, ICharacterRepository repository)
    {
        _cache = cache;
        _repository = repository;
    }
    
    public async Task<IEnumerable<PlayerCharacterData>> GetPlayerCharacterDataByUserIdAsync(int? userId)
    {
        if (userId == null)
            return Enumerable.Empty<PlayerCharacterData>();
        
        var playerCharacters = (await _repository.GetPlayerCharacterDataByUserIdAsync(userId))
            .ToList();

        foreach (var pc in playerCharacters)
        {
            var baseData = _cache.GetCharacter(pc.CharacterId);
            if (baseData == null)
                throw new RequestException(ErrorStatusCode.NotFound, "CHARACTER_NOT_FOUND",
                    new { CharacterId = pc.CharacterId });
            
            pc.CharacterName = baseData.CharacterName;
            pc.Rarity = baseData.Rarity;
        }
        
        return playerCharacters;
    }

    public async Task<PlayerCharacterData> AddPlayerCharacterDataByUserIdAsync(PlayerCharacterData data)
    {
        var baseData = _cache.GetCharacter(data.CharacterId);
        if (baseData == null)
            throw new RequestException(ErrorStatusCode.NotFound, "CHARACTER_NOT_FOUND",
                new { CharacterId = data.CharacterId });
        
        var exists = await _repository.IsNewCharacterExistAsync(data.UserId, data.CharacterId);
        if (exists)
        {
            if (data.PieceAmount > 0)
                await _repository.AddCharacterPiecesAsync(data.UserId, data.CharacterId, 10);
            
            var playerCharacters = await _repository.GetPlayerCharacterDataByUserIdAsync(data.UserId);
            var existing = playerCharacters.FirstOrDefault(c => c.CharacterId == data.CharacterId)
                           ?? throw new RequestException(ErrorStatusCode.NotFound, "CHARACTER_NOT_FOUND",
                               new { CharacterId = data.CharacterId });
            
            existing.CharacterName = baseData.CharacterName;
            existing.Rarity = baseData.Rarity;

            return existing;
        }
        else
        {
            var newCharacter = new PlayerCharacterData
            {
                UserId = data.UserId,
                CharacterId = data.CharacterId
            };

            var inserted = await _repository.AddPlayerCharacterDataByUserIdAsync(newCharacter);

            inserted.CharacterName = baseData.CharacterName;
            inserted.Rarity = baseData.Rarity;

            return inserted;
        }
    }
}