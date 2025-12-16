using Server.Application.Port;
using Server.Application.UseCase.Currency.Command;
using Server.Application.UseCase.Currency.Result;

namespace Server.Application.UseCase.Currency;

public class GetActionPointService : IGetActionPointUseCase
{
    private readonly ICurrencyRepository _repository;

    public GetActionPointService(
        ICurrencyRepository repository
        )
    {
        _repository = repository;
    }
    
    public async Task<GetActionPointResult> ExecuteAsync(GetActionPointCommand request)
    {
        var data = await _repository.FindByUserIdAsync(request.UserId);
        
        return new GetActionPointResult(
            ActionPoint:data.ActionPoint
            );
    }
}