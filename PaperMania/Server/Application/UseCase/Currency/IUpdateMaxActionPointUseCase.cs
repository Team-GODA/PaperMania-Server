using Server.Application.UseCase.Currency.Command;

namespace Server.Application.UseCase.Currency;

public interface IUpdateMaxActionPointUseCase
{
    Task ExecuteAsync(UpdateMaxActionPointCommand request);
}