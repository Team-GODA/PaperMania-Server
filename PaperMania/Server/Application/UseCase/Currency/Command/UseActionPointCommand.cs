namespace Server.Application.UseCase.Currency.Command;

public record UseActionPointCommand(
    int UserId,
    int UsedActionPoint
    );