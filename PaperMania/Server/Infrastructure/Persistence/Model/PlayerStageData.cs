namespace Server.Infrastructure.Persistence.Model;

public class PlayerStageData
{
    public int? UserId { get; set; }
    public int StageNum { get; set; }
    public int StageSubNum { get; set; }
    public bool IsCleared { get; set; }
}