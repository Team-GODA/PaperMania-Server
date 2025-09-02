using Microsoft.Extensions.Options;
using Server.Application.Configure;
using Server.Domain.Entity;

namespace Server.Infrastructure.Service;

public class StageRewardCache
{
    private readonly string _url;
    private Dictionary<(int stageNum, int stageSubNum), StageReward> _rewards = new();

    public StageRewardCache(IOptions<GoogleSheetSetting> options)
    {
        _url = options.Value.StageRewardUrl;
    }
    
    public StageReward? GetStageReward(int stageNum, int stageSubNum)
    {
        return _rewards.TryGetValue((stageNum, stageSubNum), out var reward) ? reward : null;
    }
    
    public async Task Initialize()
    {
        var stageRewardsDict = await CsvLoader.LoadCsvAsync<(int,int), StageReward>(
            _url,
            r => (r.StageNum, r.StageSubNum));
        _rewards.Clear();

        _rewards = stageRewardsDict;
    }
}