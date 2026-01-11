using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.Input.Currency;
using Server.Application.Port.Output.Infrastructure;
using Server.Application.Port.Output.Persistence;
using Server.Application.UseCase.Currency.Command;
using Server.Application.UseCase.Currency.Result;

namespace Server.Application.UseCase.Currency;

public class GainPaperPieceUseCase : IGainPaperPieceUseCase
{
    private readonly ICurrencyRepository _repository;
    private readonly ITransactionScope _transactionScope;
    
    public GainPaperPieceUseCase(
        ICurrencyRepository repository,
        ITransactionScope transactionScope
        )
    {
        _repository = repository;
        _transactionScope = transactionScope;
    }
    
    public async Task<GainPaperPieceResult> ExecuteAsync(GainPaperPieceCommand request)
    {
        request.Validate();

        var data = await _repository.FindByUserIdAsync(request.UserId)
                   ?? throw new RequestException(
                       ErrorStatusCode.NotFound,
                       "CURRENCY_DATA_NOT_FOUND");
            
        data.GainPaperPiece(request.PaperPiece);
            
        await _repository.UpdateAsync(data);
            
        return new GainPaperPieceResult(data.PaperPiece);
    }
    
    public async Task<GainPaperPieceResult>  ExecuteWithTransactionAsync(GainPaperPieceCommand request)
    {
        return await _transactionScope.ExecuteAsync(async () =>
            await ExecuteAsync(request)
        );
    }
}