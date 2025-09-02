using Server.Domain.Entity;

namespace Server.Infrastructure.Service;

public class StageRewardCache
{
    private readonly string _url =
        "https://docs.google.com/spreadsheets/d/e/2PACX-1vRjqte_1Pq7_fnmlqnJ_aoZ3xLDyZPuTP83L1buqjstcuk9nkZpVXUw0wYt2wqfI631jrdC4lTweZ2V/pub?output=csv";
    private readonly Dictionary<(int stageNum, int stageSubNum), StageReward> _rewards = new();
    
    public StageReward? GetStageReward(int stageNum, int stageSubNum)
    {
        return _rewards.TryGetValue((stageNum, stageSubNum), out var reward) ? reward : null;
    }
    
    public async Task Initialize()
    {
        var stageRewardsDict = await CsvLoader.LoadCsvAsync<(int,int), StageReward>(_url, r => (r.StageNum, r.StageSubNum));
        _rewards.Clear();
        foreach (var kv in stageRewardsDict)
            _rewards[kv.Key] = kv.Value;
    }
}