using Server.Application.UseCase.Currency.Command;

namespace Server.Application.UseCase.Currency;

public interface IRegenerateActionPointUseCase
{
    Task ExecuteAsync(RegenerateActionPointCommand request);
}