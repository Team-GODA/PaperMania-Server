using Server.Api.Dto.Response;
using Server.Application.Port;
using Server.Application.UseCase.Currency.Command;
using Server.Application.Exceptions;

namespace Server.Application.UseCase.Currency;

public class RegenerateActionPointService : IRegenerateActionPointUseCase
{
    private readonly ICurrencyRepository _repository;
    private readonly ITransactionScope _transactionScope;
    
    private const int MaxActionPoints = 100;
    private const int RegenerationAmount = 1;
    private static readonly TimeSpan RegenerationInterval = TimeSpan.FromMinutes(5);
    
    public RegenerateActionPointService(
        ICurrencyRepository repository,
        ITransactionScope transactionScope)
    {
        _repository = repository;
        _transactionScope = transactionScope;
    }
    
    public async Task ExecuteAsync(RegenerateActionPointCommand request)
    {
        var data = await _repository.FindByUserIdAsync(request.UserId);
        
        if (data == null)
            throw new RequestException(
                ErrorStatusCode.NotFound,
                "CURRENCY_DATA_NOT_FOUND");
    }
}