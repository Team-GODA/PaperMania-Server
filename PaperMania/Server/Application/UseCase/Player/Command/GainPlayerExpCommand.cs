namespace Server.Application.UseCase.Player.Command;

public record GainPlayerExpCommand(
    int? UserId,
    int Exp
    );