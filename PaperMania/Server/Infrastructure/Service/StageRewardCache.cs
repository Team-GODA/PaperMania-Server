using Microsoft.Extensions.Options;
using Server.Application.Configure;
using Server.Domain.Entity;
using Server.Infrastructure.StaticData;

namespace Server.Infrastructure.Service;

public class StageRewardCache
{
    private const string Url =
        "https://docs.google.com/spreadsheets/d/e/2PACX-1vRjqte_1Pq7_fnmlqnJ_aoZ3xLDyZPuTP83L1buqjstcuk9nkZpVXUw0wYt2wqfI631jrdC4lTweZ2V/pub?output=csv";
    private Dictionary<(int stageNum, int stageSubNum), StageReward> _rewards = new();

    public StageReward? GetStageReward(int stageNum, int stageSubNum)
    {
        return _rewards.TryGetValue((stageNum, stageSubNum), out var reward) ? reward : null;
    }
    
    public async Task Initialize()
    {
        var stageRewardsDict = await CsvHelper.LoadCsvAsync<(int,int), StageReward>(
            Url,
            r => (r.StageNum, r.StageSubNum));
        _rewards.Clear();

        _rewards = stageRewardsDict;
    }
}