// using Microsoft.Extensions.Options;
// using Server.Application.Configure;
// using Server.Domain.Entity;
// using Server.Infrastructure.StaticData;
//
// namespace Server.Infrastructure.Service;
//
// public class CharacterDataCache
// {
//     private const string Url =
//         "https://docs.google.com/spreadsheets/d/e/2PACX-1vT-hipnb7-1L4DFLEZQdC5S7MmB0DRUhn6-unGKKlL_djdIExwoQPtTv72W6e0CP9uHKe8nhIDGHPK5/pub?output=csv";
//     private Dictionary<string, CharacterData> _characters = new();
//
//     public async Task Initialize()
//     {
//         var characterDataDict = await CsvHelper.LoadCsvAsync<string, CharacterData>(
//             Url,Url
//             keySelector: c => c.CharacterId
//         );
//         
//         _characters =  characterDataDict;
//     }
//     
//     public CharacterData? GetCharacter(string characterId)
//     {
//         return _characters.TryGetValue(characterId, out var character) ? character : null;
//     }
// }