using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.In.Currency;
using Server.Application.Port.Out.Infrastructure;
using Server.Application.Port.Out.Persistence;
using Server.Application.UseCase.Currency.Command;

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
    
    public async Task<int> ExecuteAsync(SpendGoldCommand request)
    {
        request.Validate();

        return await _transactionScope.ExecuteAsync(async () =>
        {
            var data = await _repository.FindByUserIdAsync(request.UserId)
                       ?? throw new RequestException(
                           ErrorStatusCode.NotFound,
                           "PLAYER_NOT_FOUND");

            data.SpendGold(request.Gold);

            await _repository.UpdateAsync(data);

            return data.Gold;
        });
    }
}