namespace Server.Application.UseCase.Currency.Command;

public record UpdateMaxActionPointCommand(
    int UserId,
    int MaxActionPoint
    );