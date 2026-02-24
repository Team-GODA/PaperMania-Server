namespace Server.Infrastructure.Persistence.Model;

public record PlayerStageData(
    int? UserId, 
    int StageNum, 
    int StageSubNum
);