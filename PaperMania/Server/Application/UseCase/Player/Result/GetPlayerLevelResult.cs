namespace Server.Application.UseCase.Player.Result;

public record GetPlayerLevelResult(
    int Level,
    int Exp,
    int MaxExp
    );