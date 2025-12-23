using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.Input.Player;
using Server.Application.Port.Output.Infrastructure;
using Server.Application.Port.Output.Persistence;
using Server.Application.Port.Output.StaticData;
using Server.Application.UseCase.Player.Command;
using Server.Application.UseCase.Player.Result;

namespace Server.Application.UseCase.Player;

public class GainPlayerExpUseCase : IGainPlayerExpUseCase
{
    private readonly IDataDao _dataDao;
    private readonly ICurrencyDao _currencyDao;
    private readonly ILevelDefinitionStore _store;
    private readonly ITransactionScope _transactionScope;

    public GainPlayerExpUseCase(
        IDataDao dataDao,
        ICurrencyDao currencyDao,
        ILevelDefinitionStore store,
        ITransactionScope transactionScope)
    {
        _dataDao = dataDao;
        _currencyDao = currencyDao;
        _store = store;
        _transactionScope = transactionScope;
    }
    
    public async Task<GainPlayerExpUseCaseResult> ExecuteAsync(GainPlayerExpCommand request)
    {
        request.Validate();

        var data = await _dataDao.FindByUserIdAsync(request.UserId)
                   ?? throw new RequestException(
                       ErrorStatusCode.NotFound,
                       "PLAYER_DATA_NOT_FOUND"
                   );

        data.Exp += request.Exp;

        var currencyData = await _currencyDao.FindByUserIdAsync(request.UserId)
                            ?? throw new RequestException(
                                ErrorStatusCode.NotFound,
                                "PLAYER_CURRENCY_DATA_NOT_FOUND"
                                );

        var maxExp = 0;
        while (true)
        {
            var levelData = _store.GetLevelDefinition(data.Level);
            if (levelData == null || data.Exp < levelData.MaxExp)
                break;
            
            data.Exp -= levelData.MaxExp;
            data.Level++;
            
            currencyData.SetMaxActionPoint(levelData.MaxActionPoint);
            currencyData.SetActionPoint(levelData.MaxActionPoint);
            
            maxExp = levelData.MaxExp;
        }

        await _dataDao.UpdatePlayerLevelAsync(request.UserId, data.Level, data.Exp);
        await _currencyDao.UpdateAsync(currencyData);
        
        return new GainPlayerExpUseCaseResult(
            data.Level,
            data.Exp,
            maxExp,
            MaxActionPoint: currencyData.MaxActionPoint
        );
    }

    public async Task<GainPlayerExpUseCaseResult> ExecuteWithTransactionAsync(GainPlayerExpCommand request)
    {
        return await _transactionScope.ExecuteAsync(async () => 
            await ExecuteAsync(request)
            );
    }
}