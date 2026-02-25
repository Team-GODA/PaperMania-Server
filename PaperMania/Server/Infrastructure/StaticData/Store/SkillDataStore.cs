using Server.Application.Port.Output.StaticData;
using Server.Infrastructure.StaticData.Model;

namespace Server.Infrastructure.StaticData.Store;

public class SkillDataStore : ISkillDataStore, IHostedService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<SkillDataStore> _logger;
    private readonly IConfiguration _configuration;

    private Dictionary<int, SkillData> _skills = new();

    public SkillDataStore(
        IHttpClientFactory httpClientFactory,
        ILogger<SkillDataStore> logger,
        IConfiguration configuration)
    {
        _httpClient = httpClientFactory.CreateClient();
        _logger = logger;
        _configuration = configuration;
    }

    public SkillData? Get(int skillId)
    {
        return _skills.TryGetValue(skillId, out var skill) ? skill : null;
    }

    public bool Contains(int skillId)
    {
        return _skills.ContainsKey(skillId);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Loading skill definitions from CSV...");

            var secretName = _configuration["StaticData:SkillDataCsvUrlSecretName"];
            if (string.IsNullOrEmpty(secretName))
                throw new InvalidOperationException("SkillDataCsvUrlSecretName is not configured");

            var url = _configuration[secretName];
            if (string.IsNullOrEmpty(url))
                throw new InvalidOperationException(
                    $"CSV URL not found. Secret name: {secretName}");

            _skills = await CsvHelper.LoadAsync<int, SkillData>(
                _httpClient,
                url,
                cols => Map(cols),
                s => s.SkillId
            );

            ValidateSkills();

            _logger.LogInformation(
                "Skill definitions loaded successfully. Count: {Count}",
                _skills.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load skill definitions");
            throw;
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }

    private static SkillData Map(string[] cols)
    {
        // 0: SkillId
        // 1: SkillName
        // 2: SkillType
        // 3: CoolTime
        // 4: ScalingType
        // 5: TargetType

        return new SkillData(
        
            int.Parse(cols[0]),
            cols[1],

            CsvHelper.ParseEnum<SkillType>(cols[2], "SkillType"),
            float.Parse(cols[3]),

            CsvHelper.ParseEnum<SkillScalingType>(cols[4], "ScalingType"),
            CsvHelper.ParseEnum<SkillTargetType>(cols[5], "TargetType")
        );
    }

    private void ValidateSkills()
    {
        foreach (var skill in _skills.Values)
        {
            if (skill.SkillId <= 0)
                throw new InvalidOperationException("SkillId must be greater than 0");

            if (skill.CoolTime < 0)
                throw new InvalidOperationException(
                    $"Invalid CoolTime for SkillId={skill.SkillId}");

            if (skill.SkillType == SkillType.Support &&
                skill.TargetType == SkillTargetType.Enemy)
            {
                throw new InvalidOperationException(
                    $"Support skill cannot target enemy. SkillId={skill.SkillId}");
            }
        }
    }
}