using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.Input.Currency;
using Server.Application.Port.Output.Persistence;
using Server.Application.UseCase.Currency.Command;
using Server.Application.UseCase.Currency.Result;

namespace Server.Application.UseCase.Currency;

public class GetCurrencyDataUseCase : IGetCurrencyDataUseCase
{
    private readonly ICurrencyDao _dao;

    public GetCurrencyDataUseCase(ICurrencyDao dao)
    {
        _dao = dao;
    }
    
    public async Task<GetCurrencyDataResult> ExecuteAsync(GetCurrencyDataCommand request)
    {
        request.Validate();
        
        var data = await _dao.FindByUserIdAsync(request.UserId)
            ?? throw new RequestException(
                ErrorStatusCode.NotFound,
                "PLAYER_NOT_FOUND"
                );

        return new GetCurrencyDataResult(
            data.ActionPoint,
            data.Gold,
            data.PaperPiece
            );
    }
}