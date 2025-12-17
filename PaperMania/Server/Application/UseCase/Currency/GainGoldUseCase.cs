using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.In.Currency;
using Server.Application.Port.Out.Persistence;
using Server.Application.UseCase.Currency.Command;

namespace Server.Application.UseCase.Currency;

public class GainGoldUseCase : IGainGoldUseCase
{
    private readonly ICurrencyRepository _repository;

    public GainGoldUseCase(ICurrencyRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<int> ExecuteAsync(GainGoldCommand request)
    {
        var data = await _repository.FindByUserIdAsync(request.UserId);
        if (data == null)
            throw new RequestException(
                ErrorStatusCode.NotFound,
                "PLAYER_NOT_FOUND");
        
        data.Gold += request.Gold;
        await _repository.UpdateDataAsync(data);
        
        return data.Gold;
    }
}