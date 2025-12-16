using Server.Api.Dto.Response;
using Server.Application.Port;
using Server.Application.UseCase.Currency.Command;
using Server.Application.Exceptions;

namespace Server.Application.UseCase.Currency;

public class RegenerateActionPointService : IRegenerateActionPointUseCase
{
    private readonly ICurrencyRepository _repository;
    private readonly ITransactionScope _transactionScope;
    
    private const int RegenerationIntervalMinutes = 4;
    private const int RegenerationAmount = 1;
    
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
        
        if (data.ActionPoint >= data.MaxActionPoint)
            return;

        var now = DateTime.UtcNow;
        var lastRegeneration = now - data.LastActionPointUpdated;
        
        var intervalsElapsed = (int)(lastRegeneration.TotalMinutes / RegenerationIntervalMinutes);
        
        if (intervalsElapsed <= 0)
            return;
        
        var pointsToRegeneration = intervalsElapsed * RegenerationAmount;
        var newActionPoint = Math.Min(
            data.MaxActionPoint,
            data.ActionPoint + pointsToRegeneration
            );
        
        var actualPointsRegenerated = newActionPoint - data.ActionPoint;
        var actualIntervalsUsed = actualPointsRegenerated / RegenerationAmount;
        var newLastUpdated = data.LastActionPointUpdated.AddMinutes(
            actualIntervalsUsed * RegenerationIntervalMinutes);
        
        await _repository.RegenerateActionPointAsync(
            request.UserId, 
            newActionPoint,
            newLastUpdated
        );
    }
}