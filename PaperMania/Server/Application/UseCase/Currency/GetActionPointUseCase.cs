using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.Input.Currency;
using Server.Application.Port.Output.Persistence;
using Server.Application.Port.Output.Service;
using Server.Application.Port.Output.Transaction;
using Server.Application.UseCase.Currency.Command;
using Server.Application.UseCase.Currency.Result;

namespace Server.Application.UseCase.Currency;

public class GetActionPointUseCase : IGetActionPointUseCase
{
    private readonly ICurrencyRepository _repository;
    private readonly IActionPointService _apService;
    private readonly ITransactionScope _transactionScope;

    public GetActionPointUseCase(
        ICurrencyRepository repository,
        IActionPointService apService,
        ITransactionScope transactionScope
        )
    {
        _repository = repository;
        _apService = apService;
        _transactionScope = transactionScope;
    }
    
    public async Task<GetActionPointResult> ExecuteAsync(GetActionPointCommand request, CancellationToken ct)
    {
        // 회복 시 행 전체를 갱신하므로, 동시에 들어온 재화 변경을 덮어쓰지 않도록 트랜잭션(행 잠금) 안에서 처리
        return await _transactionScope.ExecuteAsync(async innerCt =>
        {
            var data = await _repository.FindByUserIdAsync(request.UserId, innerCt);
            if (data == null)
                throw new RequestException(
                    ErrorStatusCode.NotFound,
                    "CURRENCY_DATA_NOT_FOUND");

            var regenerate = _apService.TryRegenerate(data, DateTime.UtcNow);
            if (regenerate)
                await _repository.UpdateAsync(data, innerCt);

            return new GetActionPointResult(data.ActionPoint);
        }, ct);
    }
}