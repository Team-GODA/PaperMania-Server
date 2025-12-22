using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.Input.Currency;
using Server.Application.Port.Output.Infrastructure;
using Server.Application.Port.Output.Persistence;
using Server.Application.UseCase.Currency.Command;
using Server.Application.UseCase.Currency.Result;

namespace Server.Application.UseCase.Currency;

public class GainGoldUseCase : IGainGoldUseCase
{
    private readonly ICurrencyDao _dao;
    private readonly ITransactionScope _transactionScope;

    public GainGoldUseCase(
        ICurrencyDao dao,
        ITransactionScope transactionScope)
    {
        _dao = dao;
        _transactionScope = transactionScope;
    }
    
    public async Task<GainGoldResult> ExecuteAsync(GainGoldCommand request)
    {
        request.Validate();

        return await _transactionScope.ExecuteAsync(async () =>
        {
            var data = await _dao.FindByUserIdAsync(request.UserId)
                       ?? throw new RequestException(
                           ErrorStatusCode.NotFound,
                           "PLAYER_NOT_FOUND");


            data.Gold += request.Gold;
            await _dao.UpdateAsync(data);

            return new GainGoldResult(data.Gold);
        });
    }
}