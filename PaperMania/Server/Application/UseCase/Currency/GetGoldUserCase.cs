using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.In.Currency;
using Server.Application.Port.Out.Persistence;
using Server.Application.UseCase.Currency.Result;

namespace Server.Application.UseCase.Currency;

public class GetGoldUseCase : IGetGoldUseCase
{
    private readonly ICurrencyRepository _repository;
    
    public GetGoldUseCase(
        ICurrencyRepository repository
        )
    {
        _repository = repository;
    }

    public async Task<GetGoldResult> ExecuteAsync(int userId)
    {
        var data = await _repository.FindByUserIdAsync(userId);
        if (data == null)
            throw new RequestException(
                ErrorStatusCode.NotFound,
                "PLAYER_NOT_FOUND)");
        
        return new GetGoldResult(data.Gold);
    }
}