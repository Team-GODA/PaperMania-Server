using Server.Domain.Entity;
using Server.Infrastructure.StaticData;
using Server.Infrastructure.StaticData.Model;

namespace Server.Api.Dto.Response.Reward;

public class ClaimStageRewardResponse
{
    public int Gold { get; set; }
    public int PaperPiece { get; set; }
    public int Level { get; set; }
    public int Exp { get; set; }
    public int MaxActionPoint { get; set; }
    public bool IsCleared { get; set; }
}