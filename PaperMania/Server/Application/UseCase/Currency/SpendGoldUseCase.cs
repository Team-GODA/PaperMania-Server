using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.Input.Currency;
using Server.Application.Port.Output.Infrastructure;
using Server.Application.Port.Output.Persistence;
using Server.Application.UseCase.Currency.Command;
using Server.Application.UseCase.Currency.Result;

namespace Server.Application.UseCase.Currency;

public class SpendGoldUseCase : ISpendGoldUseCase
{
    private readonly ICurrencyDao _dao;
    private readonly ITransactionScope _transactionScope;
    
    public SpendGoldUseCase(
        ICurrencyDao dao,
        ITransactionScope transactionScope)
    {
        _dao = dao;
        _transactionScope = transactionScope;
    }
    
    public async Task<SpendGoldResult> ExecuteAsync(SpendGoldCommand request)
    {
        request.Validate();

        return await _transactionScope.ExecuteAsync(async () =>
        {
            var data = await _dao.FindByUserIdAsync(request.UserId)
                       ?? throw new RequestException(
                           ErrorStatusCode.NotFound,
                           "PLAYER_NOT_FOUND");

            data.SpendGold(request.Gold);

            await _dao.UpdateAsync(data);

            return new SpendGoldResult(data.Gold);
        });
    }
}