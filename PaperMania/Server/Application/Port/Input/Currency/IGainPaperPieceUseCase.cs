using Server.Application.UseCase.Currency.Command;
using Server.Application.UseCase.Currency.Result;

namespace Server.Application.Port.Input.Currency;

public interface IGainPaperPieceUseCase
{
    Task<GainPaperPieceResult> ExecuteAsync(GainPaperPieceCommand request);
    Task<GainPaperPieceResult> ExecuteWithTransactionAsync(GainPaperPieceCommand request);
}