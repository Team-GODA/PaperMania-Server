// using Server.Api.Dto.Response;
// using Server.Application.Exceptions;
// using Server.Application.Port;
// using Server.Domain.Entity;
//
// namespace Server.Infrastructure.Service;
//
// public class CharacterService : ICharacterService
// {
//     private readonly CharacterDataCache _cache;
//     private readonly ICharacterDao _repository;
//
//     public CharacterService(CharacterDataCache cache, ICharacterDao repository)
//     {
//         _cache = cache;
//         _repository = repository;
//     }
//     
//     public async Task<IEnumerable<PlayerCharacterData>> GetPlayerCharacterDataByUserIdAsync(int userId)
//     {
//         var playerCharacters = (await _repository.GetPlayerCharactersDataByUserIdAsync(userId))
//             .ToList();
//
//         foreach (var pc in playerCharacters)
//         {
//             var baseData = _cache.GetCharacter(pc.Data.CharacterId);
//             if (baseData == null)
//                 throw new RequestException(ErrorStatusCode.NotFound, "CHARACTER_NOT_FOUND",
//                     new { CharacterId = pc.Data.CharacterId });
//
//             pc.Data = baseData;
//         }
//         
//         return playerCharacters;
//     }
//
//     public async Task<PlayerCharacterData> AddPlayerCharacterDataByUserIdAsync(PlayerCharacterData data)
//     {
//         var baseData = _cache.GetCharacter(data.Data.CharacterId);
//         if (baseData == null)
//             throw new RequestException(ErrorStatusCode.NotFound, "CHARACTER_NOT_FOUND",
//                 new { CharacterId = data.Data.CharacterId });
//         
//         var exists = await _repository.HasCharacterAsync(data.UserId, data.Data.CharacterId);
//         if (exists)
//         {
//             if (data.PieceAmount > 0)
//                 await _repository.AddCharacterPiecesAsync(data.UserId, data.Data.CharacterId, 10);
//             
//             var playerCharacters = await _repository.GetPlayerCharactersDataByUserIdAsync(data.UserId);
//             var existing = playerCharacters.FirstOrDefault
//                 (c => c.Data.CharacterId == data.Data.CharacterId);
//             if (existing == null)
//                 throw new RequestException(ErrorStatusCode.NotFound, "CHARACTER_NOT_FOUND",
//                     new { CharacterId = data.Data.CharacterId });
//
//             existing.Data = baseData;
//
//             return existing;
//         }
//
//         var newCharacter = new PlayerCharacterData
//         {
//             UserId = data.UserId,
//             Data = baseData
//         };
//
//         var inserted = await _repository.AddPlayerCharacterDataByUserIdAsync(newCharacter);
//         inserted.Data = baseData;
//
//         return inserted;
//     }
// }