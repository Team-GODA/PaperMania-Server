using Server.Domain.Entity;
using Server.Infrastructure.StaticData;

namespace Server.Api.Dto.Response.Reward;

public class GetStageRewardResponse
{
    public StageReward?  StageReward { get; set; }
}