using Server.Application.UseCase.Currency.Command;
using Server.Application.UseCase.Currency.Result;

namespace Server.Application.Port.Input.Currency;

public interface IGainGoldUseCase
{
    Task<GainGoldResult> ExecuteAsync(GainGoldCommand request);
    Task<GainGoldResult> ExecuteWithTransactionAsync(GainGoldCommand request);
    
}