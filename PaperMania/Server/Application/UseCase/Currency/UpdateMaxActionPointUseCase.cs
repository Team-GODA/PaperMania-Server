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
    private readonly ICurrencyRepository _repository;
    private readonly ITransactionScope _transactionScope;

    public UpdateMaxActionPointUseCase(
        ICurrencyRepository repository,
        ITransactionScope transactionScope
    )
    {
        _repository = repository;
        _transactionScope = transactionScope;
    }
    
    public async Task<UpdateMaxActionPointResult> ExecuteAsync(UpdateMaxActionPointCommand request)
    {
        return await _transactionScope.ExecuteAsync(async () =>
        {
            var data = await _repository.FindByUserIdAsync(request.UserId);
            if (data == null)
                throw new RequestException(ErrorStatusCode.NotFound, "PLAYER_NOT_FOUND");
        
            data.MaxActionPoint = request.MaxActionPoint;
            await _repository.UpdateAsync(data);
        
            return new UpdateMaxActionPointResult(data.MaxActionPoint);
        });
    }
}