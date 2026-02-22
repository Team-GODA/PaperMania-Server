using Server.Application.UseCase.Currency.Result;

namespace Server.Application.Port.Input.Currency;

public interface IGetGoldUseCase
{
    Task<GetGoldResult> ExecuteAsync(int userId, CancellationToken ct);
}