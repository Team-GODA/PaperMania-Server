using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.Input.Currency;
using Server.Application.Port.Output.Persistence;
using Server.Application.UseCase.Currency.Command;
using Server.Application.UseCase.Currency.Result;
using Server.Domain.Service;

namespace Server.Application.UseCase.Currency;

public class GetActionPointUseCase : IGetActionPointUseCase
{
    private readonly ICurrencyDao _dao;
    private readonly ActionPointService _apService;

    public GetActionPointUseCase(
        ICurrencyDao dao,
        ActionPointService apService
        )
    {
        _dao = dao;
        _apService = apService;
    }
    
    public async Task<GetActionPointResult> ExecuteAsync(GetActionPointCommand request)
    {
        var data = await _dao.FindByUserIdAsync(request.UserId);
        if (data == null)
            throw new RequestException(
                ErrorStatusCode.NotFound,
                "CURRENCY_DATA_NOT_FOUND");
        
        var regenerate = _apService.TryRegenerate(data, DateTime.UtcNow);
        if (regenerate)
            await _dao.UpdateAsync(data);
        
        return new GetActionPointResult(data.ActionPoint);
    }
}