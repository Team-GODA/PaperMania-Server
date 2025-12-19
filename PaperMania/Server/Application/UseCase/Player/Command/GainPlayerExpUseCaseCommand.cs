namespace Server.Application.UseCase.Player.Command;

public record GainPlayerExpUseCaseCommand(
    int? UserId,
    int Exp
    );