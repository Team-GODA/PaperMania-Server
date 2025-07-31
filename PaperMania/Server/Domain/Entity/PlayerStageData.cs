namespace Server.Domain.Entity;

public class PlayerStageData
{
    public int? Id { get; set; }
    public int StageNum { get; set; }
    public int SubStageNum { get; set; }
    public bool IsCleared { get; set; }
}