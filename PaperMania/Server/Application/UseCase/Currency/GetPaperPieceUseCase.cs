using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.Input.Currency;
using Server.Application.Port.Output.Persistence;
using Server.Application.UseCase.Currency.Command;
using Server.Application.UseCase.Currency.Result;

namespace Server.Application.UseCase.Currency;

public class GetPaperPieceUseCase : IGetPaperPieceUseCase
{
    private readonly ICurrencyRepository _repository;
    
    public GetPaperPieceUseCase(ICurrencyRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<GetPaperPieceResult> ExecuteAsync(GetPaperPieceCommand request, CancellationToken ct)
    {
        request.Validate();
        
        var data = await _repository.FindByUserIdAsync(request.UserId, ct)
                   ?? throw new RequestException(
                       ErrorStatusCode.NotFound,
                       "CURRENCY_DATA_NOT_FOUND");
        
        return new GetPaperPieceResult(data.PaperPiece);
    }
}