namespace Server.Application.UseCase.Currency.Result;

public record GetCurrencyDataResult(
    int ActionPoint,
    int Gold,
    int PaperPiece
    );