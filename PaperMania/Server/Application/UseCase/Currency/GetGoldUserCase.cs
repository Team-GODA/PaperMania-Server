using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.Input.Currency;
using Server.Application.Port.Output.Persistence;
using Server.Application.UseCase.Currency.Result;

namespace Server.Application.UseCase.Currency;

public class GetGoldUseCase : IGetGoldUseCase
{
    private readonly ICurrencyDao _dao;
    
    public GetGoldUseCase(
        ICurrencyDao dao
        )
    {
        _dao = dao;
    }

    public async Task<GetGoldResult> ExecuteAsync(int userId)
    {
        var data = await _dao.FindByUserIdAsync(userId);
        if (data == null)
            throw new RequestException(
                ErrorStatusCode.NotFound,
                "PLAYER_NOT_FOUND)");
        
        return new GetGoldResult(data.Gold);
    }
}