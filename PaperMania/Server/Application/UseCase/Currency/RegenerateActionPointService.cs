using Server.Application.Port;
using Server.Application.UseCase.Currency.Command;

namespace Server.Application.UseCase.Currency;

public class RegenerateActionPointService : IRegenerateActionPointUseCase
{
    private readonly ICurrencyRepository _repository;
    private readonly ITransactionScope _transactionScope;
    
    public RegenerateActionPointService(
        ICurrencyRepository repository,
        ITransactionScope transactionScope
        )
    {
        _repository = repository;
        _transactionScope = transactionScope;
    }
    
    public async Task ExecuteAsync(RegenerateActionPointCommand request)
    {
        var data = await _repository.FindPlayerCurrencyDataByUserIdAsync(request.UserId);
        
        await _transactionScope.ExecuteAsync(async () =>
        {
            
        });
    }
}