using Server.Application.UseCase.Currency.Command;

namespace Server.Application.Port.In.Currency;

public interface IGainGoldUseCase
{
    Task<int> ExecuteAsync(GainGoldCommand request);
}