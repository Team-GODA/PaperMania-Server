using Server.Application.Port.Output.StaticData;
using Server.Infrastructure.StaticData.Model;

namespace Server.Infrastructure.StaticData.Store;

public class LevelDefinitionStore : ILevelDefinitionStore,  IHostedService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<StageRewardStore> _logger;
    private readonly IConfiguration _configuration;
    private Dictionary<int, LevelDefinition> _level = new();
    
    public LevelDefinitionStore(
        IHttpClientFactory httpClientFactory,
        ILogger<StageRewardStore> logger,
        IConfiguration configuration)
    {
        _httpClient = httpClientFactory.CreateClient();
        _logger = logger;
        _configuration = configuration;
    }
    
    public LevelDefinition? GetLevelDefinition(int level)
    {
        return _level.TryGetValue(level, out var levelDefinition) ? levelDefinition : null;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Loading stage rewards from CSV...");
            
            var secretName = _configuration["StaticData:LevelDefinitionCsvUrlSecretName"];
            if (string.IsNullOrEmpty(secretName))
                throw new InvalidOperationException("LevelDefinitionCsvUrlSecretName is not configured");
            
            var url = _configuration[secretName];
            if (string.IsNullOrEmpty(url))
                throw new InvalidOperationException($"CSV URL not found in Key Vault. Secret name: {secretName}");
            
            
            _level = await CsvHelper.LoadAsync<int, LevelDefinition>(
                _httpClient,
                url,
                cols => new LevelDefinition
                {
                    Level = int.Parse(cols[0]),
                    MaxExp = int.Parse(cols[1]),
                    MaxActionPoint =  int.Parse(cols[2])
                },
                l => l.Level
            );
            
            _logger.LogInformation("Level Definition loaded successfully. Count: {Count}", _level.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load Level Definition");
            throw;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}