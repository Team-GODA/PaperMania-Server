using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.Input.Currency;
using Server.Application.Port.Output.Persistence;
using Server.Application.UseCase.Currency.Command;
using Server.Application.UseCase.Currency.Result;

namespace Server.Application.UseCase.Currency;

public class GetPaperPieceUseCase : IGetPaperPieceUseCase
{
    private readonly ICurrencyDao _dao;
    
    public GetPaperPieceUseCase(ICurrencyDao dao)
    {
        _dao = dao;
    }
    
    public async Task<GetPaperPieceResult> ExecuteAsync(GetPaperPieceCommand request)
    {
        request.Validate();
        
        var data = await _dao.FindByUserIdAsync(request.UserId)
                   ?? throw new RequestException(
                       ErrorStatusCode.NotFound,
                       "CURRENCY_DATA_NOT_FOUND");
        
        return new GetPaperPieceResult(data.PaperPiece);
    }
}