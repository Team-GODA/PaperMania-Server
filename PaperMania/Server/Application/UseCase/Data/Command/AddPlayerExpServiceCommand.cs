namespace Server.Application.UseCase.Data.Command;

public record AddPlayerExpServiceCommand(
    int? UserId,
    int Exp
    );