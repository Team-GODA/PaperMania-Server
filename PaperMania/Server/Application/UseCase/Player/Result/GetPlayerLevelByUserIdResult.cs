namespace Server.Application.UseCase.Data.Result;

public record GetPlayerLevelByUserIdResult(
    int Level,
    int Exp
    );