namespace Server.Application.UseCase.Player.Result;

public record GainPlayerExpUseCaseResult(
    int Level,
    int Exp,
    int MaxExp,
    int MaxActionPoint
    );