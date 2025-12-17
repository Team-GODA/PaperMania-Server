using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port;
using Server.Application.Port.Out.Infrastructure;
using Server.Application.Port.Out.Persistence;
using Server.Application.UseCase.Currency.Command;
using Server.Application.UseCase.Currency.Result;

namespace Server.Application.UseCase.Currency;

public class SpendActionPointUseCase
{
    private readonly ICurrencyRepository _repository;
    private readonly ITransactionScope _transactionScope;

    public SpendActionPointUseCase(
        ICurrencyRepository repository,
        ITransactionScope transactionScope
    )
    {
        _repository = repository;
        _transactionScope = transactionScope;
    }
    
    public async Task<UseActionPointResult> ExecuteAsync(UseActionPointCommand request)
    {
        return await _transactionScope.ExecuteAsync(async () =>
        {
            var data = await _repository.FindByUserIdAsync(request.UserId);
            if (data == null)
                throw new RequestException(
                    ErrorStatusCode.NotFound,
                    "PLAYER_NOT_FOUND");

            if (data.ActionPoint < request.UsedActionPoint)
                throw new RequestException(
                    ErrorStatusCode.BadRequest,
                    "INSUFFICIENT_ACTION_POINT");
            
            data.ActionPoint = Math.Max(data.ActionPoint - request.UsedActionPoint, 0);
            data.LastActionPointUpdated = DateTime.UtcNow;
            
            await _repository.UpdateDataAsync(data);

            return new UseActionPointResult(
                data.ActionPoint
                );
        });
    }
}