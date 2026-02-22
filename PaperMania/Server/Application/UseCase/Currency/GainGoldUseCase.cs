using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.Input.Currency;
using Server.Application.Port.Output.Persistence;
using Server.Application.Port.Output.Transaction;
using Server.Application.UseCase.Currency.Command;
using Server.Application.UseCase.Currency.Result;

namespace Server.Application.UseCase.Currency;

public class GainGoldUseCase : IGainGoldUseCase
{
    private readonly ICurrencyRepository _repository;
    private readonly ITransactionScope _transactionScope;

    public GainGoldUseCase(
        ICurrencyRepository repository,
        ITransactionScope transactionScope)
    {
        _repository = repository;
        _transactionScope = transactionScope;
    }
    
    public async Task<GainGoldResult> ExecuteAsync(GainGoldCommand request, CancellationToken ct)
    {
        request.Validate();

        var data = await _repository.FindByUserIdAsync(request.UserId, ct)
                   ?? throw new RequestException(
                       ErrorStatusCode.NotFound,
                       "PLAYER_NOT_FOUND");


        data.Gold += request.Gold;
        await _repository.UpdateAsync(data, ct);

        return new GainGoldResult(data.Gold);
    }
    
    public async Task<GainGoldResult> ExecuteWithTransactionAsync(GainGoldCommand request, CancellationToken ct)
    {
        return await _transactionScope.ExecuteAsync(async (innerCt) =>
            await ExecuteAsync(request, innerCt)
            , ct);
    }
}