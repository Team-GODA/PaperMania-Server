using Microsoft.Extensions.Options;
using Server.Application.Configure;
using Server.Domain.Entity;

namespace Server.Infrastructure.Service;

public class CharacterDataCache
{
    private readonly string _url;
    private Dictionary<string, CharacterData> _characters = new();

    public CharacterDataCache(IOptions<GoogleSheetSetting> options)
    {
        _url = options.Value.CharacterDataUrl
               ?? throw new ArgumentNullException(nameof(options.Value.CharacterDataUrl));
    }
    
    public async Task Initialize()
    {
        var characterDataDict = await CsvLoader.LoadCsvAsync<string, CharacterData>(
            _url,
            keySelector: c => c.CharacterId
        );
        
        _characters =  characterDataDict;
    }
    
    public CharacterData? GetCharacter(string characterId)
    {
        return _characters.TryGetValue(characterId, out var character) ? character : null;
    }
}