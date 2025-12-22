using Server.Domain.Entity;
using Server.Infrastructure.StaticData;

namespace Server.Api.Dto.Response.Reward;

public class ClaimStageRewardResponse
{
    public int? Id   { get; set; }
    public StageReward? StageReward { get; set; }
}