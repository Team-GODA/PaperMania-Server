using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.Input.Currency;
using Server.Application.Port.Output.Infrastructure;
using Server.Application.Port.Output.Persistence;
using Server.Application.UseCase.Currency.Command;
using Server.Application.UseCase.Currency.Result;

namespace Server.Application.UseCase.Currency;

public class SpendActionPointUseCase : ISpendActionPointUseCase
{
    private readonly ICurrencyDao _dao;
    private readonly ITransactionScope _transactionScope;

    public SpendActionPointUseCase(
        ICurrencyDao dao,
        ITransactionScope transactionScope
    )
    {
        _dao = dao;
        _transactionScope = transactionScope;
    }
    
    public async Task<SpendActionPointResult> ExecuteAsync(SpendActionPointCommand request)
    {
        request.Validate();
        
        return await _transactionScope.ExecuteAsync(async () =>
        {
            var data = await _dao.FindByUserIdAsync(request.UserId);
            if (data == null)
                throw new RequestException(
                    ErrorStatusCode.NotFound,
                    "PLAYER_NOT_FOUND");

            if (data.ActionPoint < request.ActionPoint)
                throw new RequestException(
                    ErrorStatusCode.BadRequest,
                    "INSUFFICIENT_ACTION_POINT");
            
            data.ActionPoint = Math.Max(data.ActionPoint - request.ActionPoint, 0);
            data.LastActionPointUpdated = DateTime.UtcNow;
            
            await _dao.UpdateAsync(data);

            return new SpendActionPointResult(
                data.ActionPoint
                );
        });
    }
}