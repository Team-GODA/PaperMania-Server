using Server.Api.Dto.Response;
using Server.Application.Port;
using Server.Application.UseCase.Currency.Command;
using Server.Application.Exceptions;
using Server.Domain.Service;

namespace Server.Application.UseCase.Currency;

public class RegenerateActionPointUseCase : IRegenerateActionPointUseCase
{
    private readonly ICurrencyRepository _repository;
    private readonly ActionPointService _apService;
    
    public RegenerateActionPointUseCase(
        ICurrencyRepository repository,
        ActionPointService apService
        )
    {
        _repository = repository;
        _apService = apService;
    }

    public async Task ExecuteAsync(RegenerateActionPointCommand request)
    {
        var data = await _repository.FindByUserIdAsync(request.UserId)
                   ?? throw new RequestException(
                       ErrorStatusCode.NotFound,
                       "CURRENCY_DATA_NOT_FOUND");

        if (!_apService.TryRegenerate(data, DateTime.UtcNow))
            return;

        await _repository.RegenerateActionPointAsync(
            request.UserId,
            data.ActionPoint,
            data.LastActionPointUpdated
        );
    }
}
