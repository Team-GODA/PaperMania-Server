using Server.Application.UseCase.Currency.Command;
using Server.Application.UseCase.Currency.Result;

namespace Server.Application.Port.Input.Currency;

public interface IGainGoldUseCase
{
    Task<GainGoldResult> ExecuteAsync(GainGoldCommand request, CancellationToken ct);
    Task<GainGoldResult> ExecuteWithTransactionAsync(GainGoldCommand request, CancellationToken ct);
}