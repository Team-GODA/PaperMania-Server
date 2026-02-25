using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.Input.Currency;
using Server.Application.Port.Output.Persistence;
using Server.Application.Port.Output.Service;
using Server.Application.UseCase.Currency.Command;
using Server.Application.UseCase.Currency.Result;

namespace Server.Application.UseCase.Currency;

public class GetActionPointUseCase : IGetActionPointUseCase
{
    private readonly ICurrencyRepository _repository;
    private readonly IActionPointService _apService;

    public GetActionPointUseCase(
        ICurrencyRepository repository,
        IActionPointService apService
        )
    {
        _repository = repository;
        _apService = apService;
    }
    
    public async Task<GetActionPointResult> ExecuteAsync(GetActionPointCommand request, CancellationToken ct)
    {
        var data = await _repository.FindByUserIdAsync(request.UserId, ct);
        if (data == null)
            throw new RequestException(
                ErrorStatusCode.NotFound,
                "CURRENCY_DATA_NOT_FOUND");
        
        var regenerate = _apService.TryRegenerate(data, DateTime.UtcNow);
        if (regenerate)
            await _repository.UpdateAsync(data, ct);
        
        return new GetActionPointResult(data.ActionPoint);
    }
}