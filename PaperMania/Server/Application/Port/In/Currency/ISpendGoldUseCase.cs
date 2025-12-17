using Server.Application.UseCase.Currency.Command;

namespace Server.Application.Port.In.Currency;

public interface ISpendGoldUseCase
{
    Task<int> ExecuteAsync(SpendGoldCommand request);
}