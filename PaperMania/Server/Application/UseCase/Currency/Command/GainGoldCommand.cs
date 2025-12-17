namespace Server.Application.UseCase.Currency.Command;

public record GainGoldCommand(
    int UserId,
    int Gold
    );