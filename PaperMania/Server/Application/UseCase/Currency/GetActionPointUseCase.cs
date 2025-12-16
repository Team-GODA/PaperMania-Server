using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port;
using Server.Application.UseCase.Currency.Command;
using Server.Application.UseCase.Currency.Result;

namespace Server.Application.UseCase.Currency;

public class GetActionPointUseCase : IGetActionPointUseCase
{
    private readonly ICurrencyRepository _repository;

    public GetActionPointUseCase(
        ICurrencyRepository repository
        )
    {
        _repository = repository;
    }
    
    public async Task<GetActionPointResult> ExecuteAsync(GetActionPointCommand request)
    {
        var data = await _repository.FindByUserIdAsync(request.UserId);
        if (data == null)
            throw new RequestException(
                ErrorStatusCode.NotFound,
                "CURRENCY_DATA_NOT_FOUND");
        
        return new GetActionPointResult(
            ActionPoint:data.ActionPoint
            );
    }
}