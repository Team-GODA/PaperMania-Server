using Server.Application.Port.Output.StaticData;

namespace Server.Infrastructure.StaticData.Store;

public class StageRewardStore : IStageRewardStore, IHostedService
{
    private const string Url =
        "https://docs.google.com/spreadsheets/d/e/2PACX-1vRjqte_1Pq7_fnmlqnJ_aoZ3xLDyZPuTP83L1buqjstcuk9nkZpVXUw0wYt2wqfI631jrdC4lTweZ2V/pub?output=csv";
    
    private readonly HttpClient _httpClient;
    private readonly ILogger<StageRewardStore> _logger;
    private Dictionary<(int stageNum, int stageSubNum), StageReward> _rewards = new();

    public StageRewardStore(
        IHttpClientFactory httpClientFactory,
        ILogger<StageRewardStore> logger)
    {
        _httpClient = httpClientFactory.CreateClient();
        _logger = logger;
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
            
            _rewards = await CsvHelper.LoadAsync<(int, int), StageReward>(
                _httpClient,
                Url,
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