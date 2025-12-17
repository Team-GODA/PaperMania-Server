namespace Server.Application.UseCase.Player.Command;

public record AddPlayerExpServiceCommand(
    int? UserId,
    int Exp
    );