using Server.Application.UseCase.Currency.Command;
using Server.Application.UseCase.Currency.Result;

namespace Server.Application.Port.In.Currency;

public interface ISpendPaperPieceUseCase
{
    Task<SpendPaperPieceResult> ExecuteAsync(SpendPaperPieceCommand request);
}