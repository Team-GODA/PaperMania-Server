using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.Input.Player;
using Server.Application.Port.Output.Persistence;
using Server.Application.Port.Output.StaticData;
using Server.Application.Port.Output.Transaction;
using Server.Application.UseCase.Player.Command;
using Server.Application.UseCase.Player.Result;

namespace Server.Application.UseCase.Player;

public class GainPlayerExpUseCase : IGainPlayerExpUseCase
{
    private readonly IDataRepository _dataRepository;
    private readonly ICurrencyRepository _currencyRepository;
    private readonly ILevelDefinitionStore _store;
    private readonly ITransactionScope _transactionScope;

    public GainPlayerExpUseCase(
        IDataRepository dataRepository,
        ICurrencyRepository currencyRepository,
        ILevelDefinitionStore store,
        ITransactionScope transactionScope)
    {
        _dataRepository = dataRepository;
        _currencyRepository = currencyRepository;
        _store = store;
        _transactionScope = transactionScope;
    }
    
    public async Task<GainPlayerExpUseCaseResult> ExecuteAsync(GainPlayerExpCommand request, CancellationToken ct)
    {
        request.Validate();

        var playerGame = await _dataRepository.FindByUserIdAsync(request.UserId, ct)
                         ?? throw new RequestException(ErrorStatusCode.NotFound, "PLAYER_DATA_NOT_FOUND");

        var currencyData = await _currencyRepository.FindByUserIdAsync(request.UserId, ct)
                           ?? throw new RequestException(ErrorStatusCode.NotFound, "PLAYER_CURRENCY_DATA_NOT_FOUND");

        playerGame.GainExp(request.Exp, _store, (maxAp) => 
        {
            currencyData.SetMaxActionPoint(maxAp);
            currencyData.SetActionPoint(maxAp);
        });

        await _dataRepository.UpdateAsync(playerGame, ct); 
        await _currencyRepository.UpdateAsync(currencyData, ct);

        var currentLevelDef = _store.GetLevelDefinition(playerGame.Level);

        return new GainPlayerExpUseCaseResult(
            playerGame.Level,
            playerGame.Exp,
            currentLevelDef?.MaxExp ?? 0,
            MaxActionPoint: currencyData.MaxActionPoint
        );
    }

    public async Task<GainPlayerExpUseCaseResult> ExecuteWithTransactionAsync(GainPlayerExpCommand request, CancellationToken ct)
    {
        return await _transactionScope.ExecuteAsync(async (innerCt) => 
            await ExecuteAsync(request, innerCt)
            , ct);
    }
}