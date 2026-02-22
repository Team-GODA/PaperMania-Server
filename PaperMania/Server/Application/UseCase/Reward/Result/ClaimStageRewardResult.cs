namespace Server.Application.UseCase.Reward.Result;

public record ClaimStageRewardResult(
    int Gold,
    int PaperPiece,
    int Level,
    int Exp,
    int MaxActionPoint,
    bool IsCleared
    );