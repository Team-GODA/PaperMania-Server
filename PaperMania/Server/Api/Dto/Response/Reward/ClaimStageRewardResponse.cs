using Server.Domain.Entity;
using Server.Infrastructure.StaticData;
using Server.Infrastructure.StaticData.Model;

namespace Server.Api.Dto.Response.Reward;

public class ClaimStageRewardResponse
{
    public int? Id   { get; set; }
    public StageReward? StageReward { get; set; }
}