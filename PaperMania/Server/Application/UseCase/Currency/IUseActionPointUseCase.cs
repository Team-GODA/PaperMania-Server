using Server.Application.UseCase.Currency.Command;

namespace Server.Application.UseCase.Currency;

public interface IUseActionPointUseCase
{
    Task ExecuteAsync(UseActionPointCommand request);
}