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
    private readonly ICurrencyDao _dao;
    private readonly ITransactionScope _transactionScope;
    
    public GainPaperPieceUseCase(
        ICurrencyDao dao,
        ITransactionScope transactionScope
        )
    {
        _dao = dao;
        _transactionScope = transactionScope;
    }
    
    public async Task<GainPaperPieceResult> ExecuteAsync(GainPaperPieceCommand request)
    {
        request.Validate();

        var data = await _dao.FindByUserIdAsync(request.UserId)
                   ?? throw new RequestException(
                       ErrorStatusCode.NotFound,
                       "CURRENCY_DATA_NOT_FOUND");
            
        data.GainPaperPiece(request.PaperPiece);
            
        await _dao.UpdateAsync(data);
            
        return new GainPaperPieceResult(data.PaperPiece);
    }
    
    public async Task<GainPaperPieceResult>  ExecuteWithTransactionAsync(GainPaperPieceCommand request)
    {
        return await _transactionScope.ExecuteAsync(async () =>
            await ExecuteAsync(request)
        );
    }
}