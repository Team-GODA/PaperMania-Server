using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.Input.Currency;
using Server.Application.Port.Input.Player;
using Server.Application.Port.Input.Reward;
using Server.Application.Port.Output.Persistence;
using Server.Application.Port.Output.StaticData;
using Server.Application.Port.Output.Transaction;
using Server.Application.UseCase.Currency.Command;
using Server.Application.UseCase.Player.Command;
using Server.Application.UseCase.Reward.Command;
using Server.Application.UseCase.Reward.Result;
using Server.Infrastructure.Persistence.Model;

namespace Server.Application.UseCase.Reward;
 
public class ClaimStageRewardUseCase : IClaimStageRewardUseCase
{
    private readonly IStageRepository _stageRepository;
    private readonly ICurrencyRepository _currencyRepository;
    private readonly IDataRepository _dataRepository;
    private readonly IStageRewardStore _stageRewardStore;
    private readonly ICheckStageClearedUseCase _checkStageClearedUseCase;
    private readonly IGainGoldUseCase _gainGoldUseCase;
    private readonly IGainPaperPieceUseCase _gainPaperPieceUseCase;
    private readonly IGainPlayerExpUseCase _gainPlayerExpUseCase;
    private readonly ITransactionScope _transactionScope;

    public ClaimStageRewardUseCase(
        IStageRepository stageRepository,
        ICurrencyRepository currencyRepository,
        IDataRepository dataRepository,
        IStageRewardStore stageRewardStore,
        ICheckStageClearedUseCase checkStageClearedUseCase,
        IGainGoldUseCase gainGoldUseCase,
        IGainPaperPieceUseCase gainPaperPieceUseCase,
        IGainPlayerExpUseCase gainPlayerExpUseCase,
        ITransactionScope transactionScope)
    {
        _stageRepository = stageRepository;
        _currencyRepository = currencyRepository;
        _dataRepository = dataRepository;
        _stageRewardStore = stageRewardStore;
        _checkStageClearedUseCase = checkStageClearedUseCase;
        _gainGoldUseCase = gainGoldUseCase;
        _gainPaperPieceUseCase = gainPaperPieceUseCase;
        _gainPlayerExpUseCase = gainPlayerExpUseCase;
        _transactionScope = transactionScope;
    }
    
    public async Task<ClaimStageRewardResult> ExecuteAsync(ClaimStageRewardCommand request, CancellationToken ct)
    {
        request.Validate();

        var stageReward = _stageRewardStore.GetStageReward(request.StageNum, request.StageSubNum)
                          ?? throw new RequestException(
                              ErrorStatusCode.NotFound,
                              "STAGE_REWARD_NOT_FOUND"
                          );

        return await _transactionScope.ExecuteAsync(async (innerCt) =>
        {
            var checkCommand = new CheckStageClearedCommand(
                request.UserId,
                request.StageNum,
                request.StageSubNum
            );
            var isCleared = await _checkStageClearedUseCase.ExecuteAsync(checkCommand, innerCt);
            
            if (!isCleared)
            {
                var stageData = new PlayerStageData(
                    request.UserId,
                    request.StageNum,
                    request.StageSubNum
                    );
                
                await _stageRepository.CreateAsync(stageData, innerCt);
            }
            
            var goldToGain = stageReward.Gold;
            var paperPieceToGain = !isCleared ? stageReward.PaperPiece : 0;
            var expToGain = stageReward.ClearExp;

            if (goldToGain > 0)
            {
                await _gainGoldUseCase.ExecuteAsync(
                    new GainGoldCommand(request.UserId, goldToGain),
                    innerCt
                );
            }

            if (paperPieceToGain > 0)
            {
                await _gainPaperPieceUseCase.ExecuteAsync(
                    new GainPaperPieceCommand(request.UserId, paperPieceToGain),
                    innerCt
                );
            }

            if (expToGain > 0)
            {
                await _gainPlayerExpUseCase.ExecuteAsync(
                    new GainPlayerExpCommand(request.UserId, expToGain),
                    innerCt
                );
            }

            var currencyData = await _currencyRepository.FindByUserIdAsync(request.UserId, innerCt)
                               ?? throw new RequestException(
                                   ErrorStatusCode.NotFound,
                                   "PLAYER_CURRENCY_DATA_NOT_FOUND"
                                   );

            var playerData = await _dataRepository.FindByUserIdAsync(request.UserId, innerCt)
                             ?? throw new RequestException(
                                 ErrorStatusCode.NotFound,
                                 "PLAYER_DATA_NOT_FOUND"
                                 );

            return new ClaimStageRewardResult(
                currencyData.Gold,
                currencyData.PaperPiece,
                playerData.Level,
                playerData.Exp,
                currencyData.MaxActionPoint,
                isCleared
            );
        }, ct);
    }
}