namespace Server.Application.UseCase.Reward.Result;

public record ClaimStageRewardResult(
    int Gold,
    int PaperPiece,
    int Level,
    int Exp,
    bool IsReChallenge
    );