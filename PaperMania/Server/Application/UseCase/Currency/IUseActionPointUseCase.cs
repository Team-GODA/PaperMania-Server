using Server.Application.UseCase.Currency.Command;
using Server.Application.UseCase.Currency.Result;

namespace Server.Application.UseCase.Currency;

public interface IUseActionPointUseCase
{
    Task<UseActionPointResult> ExecuteAsync(UseActionPointCommand request);
}