using Server.Application.UseCase.Currency.Command;
using Server.Application.UseCase.Currency.Result;

namespace Server.Application.Port.Input.Currency;

public interface IGetPaperPieceUseCase
{
    Task<GetPaperPieceResult> ExecuteAsync(GetPaperPieceCommand request, CancellationToken ct);
}