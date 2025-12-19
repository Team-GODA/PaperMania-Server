using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.In.Currency;
using Server.Application.Port.Out.Infrastructure;
using Server.Application.Port.Out.Persistence;
using Server.Application.UseCase.Currency.Command;
using Server.Application.UseCase.Currency.Result;

namespace Server.Application.UseCase.Currency;

public class SpendPaperPieceUseCase : ISpendPaperPieceUseCase
{
    private readonly ICurrencyRepository _repository;
    private readonly ITransactionScope _transactionScope;
    
    public SpendPaperPieceUseCase(
        ICurrencyRepository repository,
        ITransactionScope transactionScope)
    {
        _repository = repository;
        _transactionScope = transactionScope;
    }
    
    public async Task<SpendPaperPieceResult> ExecuteAsync(SpendPaperPieceCommand request)
    {
        request.Validate();

        return await _transactionScope.ExecuteAsync(async () =>
        {
            var data = await _repository.FindByUserIdAsync(request.UserId)
                       ?? throw new RequestException(
                           ErrorStatusCode.NotFound,
                           "CURRENCY_DATA_NOT_FOUND");

            data.SpendPaperPiece(request.PaperPiece);

            await _repository.UpdateAsync(data);

            return new SpendPaperPieceResult(data.PaperPiece);
        });
    }
}