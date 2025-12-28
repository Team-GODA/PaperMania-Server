using Server.Application.Port.Output.StaticData;
using Server.Infrastructure.StaticData.Model;

namespace Server.Infrastructure.StaticData.Store;

public class CharacterStore : ICharacterStore, IHostedService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CharacterStore> _logger;
    private readonly IConfiguration _configuration;
    private readonly ISkillDataStore _skillStore;

    private Dictionary<int, CharacterData> _characters = new();

    public CharacterStore(
        IHttpClientFactory httpClientFactory,
        ILogger<CharacterStore> logger,
        IConfiguration configuration,
        ISkillDataStore skillStore)
    {
        _httpClient = httpClientFactory.CreateClient();
        _logger = logger;
        _configuration = configuration;
        _skillStore = skillStore;
    }

    public CharacterData? Get(int characterId)
    {
        return _characters.TryGetValue(characterId, out var c) ? c : null;
    }

    public IReadOnlyDictionary<int, CharacterData> GetAll()
    {
        return _characters;
    }

     public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Loading character definitions from CSV...");

            var secretName = _configuration["StaticData:CharacterDataCsvUrlSecretName"];
            if (string.IsNullOrEmpty(secretName))
                throw new InvalidOperationException("CharacterDataCsvUrlSecretName is not configured");

            var url = _configuration[secretName];
            if (string.IsNullOrEmpty(url))
                throw new InvalidOperationException(
                    $"CSV URL not found. Secret name: {secretName}");
            
            _characters = await CsvHelper.LoadAsync(
                _httpClient,
                url,
                Map,
                c => c.CharacterId
            );

            ValidateCharacters();

            _logger.LogInformation(
                "Character definitions loaded successfully. Count: {Count}",
                _characters.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load character definitions");
            throw;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    } 

    private static CharacterData Map(string[] cols)
    {
        // 0: CharacterId
        // 1: CharacterName
        // 2: Role
        // 3: Rarity
        // 4: BaseHP
        // 5: BaseATK
        // 6: NormalSkillId
        // 7: UltimateSkillId
        // 8: SupportSkillId

        return new CharacterData
        {
            CharacterId = int.Parse(cols[0]),
            CharacterName = cols[1],

            Role = CsvHelper.ParseEnum<CharacterRole>(cols[2], "Role"),
            Rarity = CsvHelper.ParseEnum<CharacterRarity>(cols[3], "Rarity"),
            BaseHP = float.Parse(cols[4]),
            BaseATK = float.Parse(cols[5]),

            NormalSkillId = ParseOptionalInt(cols[6]),
            UltimateSkillId = ParseOptionalInt(cols[7]),
            SupportSkillId = ParseOptionalInt(cols[8]),

        };
    }
    
    private void ValidateCharacters()
    {
        foreach (var c in _characters.Values)
        {
            if (c.CharacterId <= 0)
                throw new InvalidOperationException("CharacterId must be greater than 0");

            if (c.BaseHP <= 0 || c.BaseATK <= 0)
                throw new InvalidOperationException(
                    $"Invalid base stat for CharacterId={c.CharacterId}");

            if (c.Role == CharacterRole.Main)
            {
                ValidateSkill(c.NormalSkillId, SkillType.Normal, c.CharacterId);
                ValidateSkill(c.UltimateSkillId, SkillType.Ultimate, c.CharacterId);
            }

            if (c.Role == CharacterRole.Support)
            {
                ValidateSkill(c.SupportSkillId, SkillType.Support, c.CharacterId);
            }
        }
    }
    
    private void ValidateSkill(int skillId, SkillType expectedType, int characterId)
    {
        if (skillId == 0)
            return;

        var skill = _skillStore.Get(skillId);
        if (skill == null)
            throw new InvalidOperationException(
                $"Invalid SkillId={skillId} for CharacterId={characterId}");

        if (skill.SkillType != expectedType)
            throw new InvalidOperationException(
                $"SkillType mismatch. CharacterId={characterId}, SkillId={skillId}");
    }
    
    private static int ParseOptionalInt(string value)
        => string.IsNullOrWhiteSpace(value) ? 0 : int.Parse(value);
}