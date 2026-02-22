using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.Input.Currency;
using Server.Application.Port.Output.Persistence;
using Server.Application.Port.Output.Transaction;
using Server.Application.UseCase.Currency.Command;
using Server.Application.UseCase.Currency.Result;

namespace Server.Application.UseCase.Currency;

public class SpendGoldUseCase : ISpendGoldUseCase
{
    private readonly ICurrencyRepository _repository;
    private readonly ITransactionScope _transactionScope;
    
    public SpendGoldUseCase(
        ICurrencyRepository repository,
        ITransactionScope transactionScope)
    {
        _repository = repository;
        _transactionScope = transactionScope;
    }
    
    public async Task<SpendGoldResult> ExecuteAsync(SpendGoldCommand request, CancellationToken ct)
    {
        request.Validate();

        return await _transactionScope.ExecuteAsync(async (innerCt) =>
        {
            var data = await _repository.FindByUserIdAsync(request.UserId, innerCt)
                       ?? throw new RequestException(
                           ErrorStatusCode.NotFound,
                           "PLAYER_NOT_FOUND");

            data.SpendGold(request.Gold);

            await _repository.UpdateAsync(data, innerCt);

            return new SpendGoldResult(data.Gold);
        }, ct);
    }
}