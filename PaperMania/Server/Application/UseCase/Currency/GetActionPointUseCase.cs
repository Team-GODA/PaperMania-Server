using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port;
using Server.Application.Port.Out.Persistence;
using Server.Application.UseCase.Currency.Command;
using Server.Application.UseCase.Currency.Result;
using Server.Domain.Service;

namespace Server.Application.UseCase.Currency;

public class GetActionPointUseCase
{
    private readonly ICurrencyRepository _repository;
    private readonly ActionPointService _apService;

    public GetActionPointUseCase(
        ICurrencyRepository repository,
        ActionPointService apService
        )
    {
        _repository = repository;
        _apService = apService;
    }
    
    public async Task<GetActionPointResult> ExecuteAsync(GetActionPointCommand request)
    {
        var data = await _repository.FindByUserIdAsync(request.UserId);
        if (data == null)
            throw new RequestException(
                ErrorStatusCode.NotFound,
                "CURRENCY_DATA_NOT_FOUND");
        
        var regenerate = _apService.TryRegenerate(data, DateTime.UtcNow);
        if (regenerate)
            await _repository.UpdateDataAsync(data);
        
        return new GetActionPointResult(data.ActionPoint);
    }
}