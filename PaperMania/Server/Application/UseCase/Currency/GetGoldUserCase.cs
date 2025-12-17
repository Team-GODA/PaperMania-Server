using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port;
using Server.Application.Port.In;
using Server.Application.Port.Out.Persistence;

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

    public async Task<int> ExecuteAsync(int userId)
    {
        var data = await _repository.FindByUserIdAsync(userId);
        if (data == null)
            throw new RequestException(
                ErrorStatusCode.NotFound,
                "PLAYER_NOT_FOUND)");
        
        return data.Gold;
    }
}