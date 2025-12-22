using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.Input.Currency;
using Server.Application.Port.Output.Infrastructure;
using Server.Application.Port.Output.Persistence;
using Server.Application.UseCase.Currency.Command;
using Server.Application.UseCase.Currency.Result;

namespace Server.Application.UseCase.Currency;

public class UpdateMaxActionPointUseCase : IUpdateMaxActionPointUseCase
{
    private readonly ICurrencyDao _dao;
    private readonly ITransactionScope _transactionScope;

    public UpdateMaxActionPointUseCase(
        ICurrencyDao dao,
        ITransactionScope transactionScope
    )
    {
        _dao = dao;
        _transactionScope = transactionScope;
    }
    
    public async Task<UpdateMaxActionPointResult> ExecuteAsync(UpdateMaxActionPointCommand request)
    {
        return await _transactionScope.ExecuteAsync(async () =>
        {
            var data = await _dao.FindByUserIdAsync(request.UserId);
            if (data == null)
                throw new RequestException(ErrorStatusCode.NotFound, "PLAYER_NOT_FOUND");
        
            data.MaxActionPoint = request.MaxActionPoint;
            await _dao.UpdateAsync(data);
        
            return new UpdateMaxActionPointResult(data.MaxActionPoint);
        });
    }
}