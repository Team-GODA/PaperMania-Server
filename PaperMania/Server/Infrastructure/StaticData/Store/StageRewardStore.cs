using Server.Application.Port.Output.StaticData;

namespace Server.Infrastructure.StaticData.Store;

public class StageRewardStore : IStageRewardStore, IHostedService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<StageRewardStore> _logger;
    private readonly IConfiguration _configuration;
    private Dictionary<(int stageNum, int stageSubNum), StageReward> _rewards = new();

    public StageRewardStore(
        IHttpClientFactory httpClientFactory,
        ILogger<StageRewardStore> logger,
        IConfiguration configuration)
    {
        _httpClient = httpClientFactory.CreateClient();
        _logger = logger;
        _configuration = configuration;
    }

    public StageReward? GetStageReward(int stageNum, int stageSubNum)
    {
        return _rewards.TryGetValue((stageNum, stageSubNum), out var reward) ? reward : null;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Loading stage rewards from CSV...");
            
            var secretName = _configuration["StaticData:StageRewardCsvUrlSecretName"];
            if (string.IsNullOrEmpty(secretName))
                throw new InvalidOperationException("StageRewardCsvUrlSecretName is not configured");
            
            var url = _configuration[secretName];
            if (string.IsNullOrEmpty(url))
                throw new InvalidOperationException($"CSV URL not found in Key Vault. Secret name: {secretName}");
            
            
            _rewards = await CsvHelper.LoadAsync<(int, int), StageReward>(
                _httpClient,
                url,
                cols => new StageReward
                {
                    StageNum = int.Parse(cols[0]),
                    StageSubNum = int.Parse(cols[1]),
                    PaperPiece = int.Parse(cols[2]),
                    Gold = int.Parse(cols[3]),
                    ClearExp = int.Parse(cols[4])
                },
                r => (r.StageNum, r.StageSubNum)
            );
            
            _logger.LogInformation("Stage rewards loaded successfully. Count: {Count}", _rewards.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load stage rewards");
            throw;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}