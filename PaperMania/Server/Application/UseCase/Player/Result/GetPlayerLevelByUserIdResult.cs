namespace Server.Application.UseCase.Player.Result;

public record GetPlayerLevelByUserIdResult(
    int Level,
    int Exp
    );