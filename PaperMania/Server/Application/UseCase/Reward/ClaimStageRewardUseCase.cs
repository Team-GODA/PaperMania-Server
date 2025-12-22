using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.Input.Currency;
using Server.Application.Port.Input.Player;
using Server.Application.Port.Input.Reward;
using Server.Application.Port.Output.Infrastructure;
using Server.Application.Port.Output.Persistence;
using Server.Application.Port.Output.StaticData;
using Server.Application.UseCase.Currency.Command;
using Server.Application.UseCase.Player.Command;
using Server.Application.UseCase.Reward.Command;
using Server.Application.UseCase.Reward.Result;
using Server.Infrastructure.Persistence.Model;

namespace Server.Application.UseCase.Reward;
 
public class ClaimStageRewardUseCase : IClaimStageRewardUseCase
{
    private readonly IStageDao _stageDao;
    private readonly ICurrencyDao _currencyDao;
    private readonly IDataDao _dataDao;
    private readonly IStageRewardStore _stageRewardStore;
    private readonly IGainGoldUseCase _gainGoldUseCase;
    private readonly IGainPaperPieceUseCase _gainPaperPieceUseCase;
    private readonly IGainPlayerExpUseCase _gainPlayerExpUseCase;
    private readonly ITransactionScope _transactionScope;

    public ClaimStageRewardUseCase(
        IStageDao stageDao,
        ICurrencyDao currencyDao,
        IDataDao dataDao,
        IStageRewardStore stageRewardStore,
        IGainGoldUseCase gainGoldUseCase,
        IGainPaperPieceUseCase gainPaperPieceUseCase,
        IGainPlayerExpUseCase gainPlayerExpUseCase,
        ITransactionScope transactionScope)
    {
        _stageDao = stageDao;
        _currencyDao = currencyDao;
        _dataDao = dataDao;
        _stageRewardStore = stageRewardStore;
        _gainGoldUseCase = gainGoldUseCase;
        _gainPaperPieceUseCase = gainPaperPieceUseCase;
        _gainPlayerExpUseCase = gainPlayerExpUseCase;
        _transactionScope = transactionScope;
    }
    
    public async Task<ClaimStageRewardResult> ExecuteAsync(ClaimStageRewardCommand request)
    {
        request.Validate();

        var stageReward = _stageRewardStore.GetStageReward(request.StageNum, request.StageSubNum)
                          ?? throw new RequestException(
                              ErrorStatusCode.NotFound,
                              "STAGE_REWARD_NOT_FOUND"
                          );

        return await _transactionScope.ExecuteAsync(async () =>
        {
            var cleared = await _stageDao.FindByUserIdAsync(
                request.UserId,
                request.StageNum,
                request.StageSubNum
            );

            var isReChallenge = cleared != null;
            
            if (!isReChallenge)
            {
                var stageData = new PlayerStageData
                {
                    UserId = request.UserId,
                    StageNum = request.StageNum,
                    StageSubNum = request.StageSubNum
                };
                await _stageDao.CreateAsync(stageData);
            }

            var goldToGain = stageReward.Gold;
            var paperPieceToGain = !isReChallenge ? stageReward.PaperPiece : 0;
            var expToGain = stageReward.ClearExp;

            if (goldToGain > 0)
            {
                await _gainGoldUseCase.ExecuteAsync(
                    new GainGoldCommand(request.UserId, goldToGain)
                );
            }

            if (paperPieceToGain > 0)
            {
                await _gainPaperPieceUseCase.ExecuteAsync(
                    new GainPaperPieceCommand(request.UserId, paperPieceToGain)
                );
            }

            if (expToGain > 0)
            {
                await _gainPlayerExpUseCase.ExecuteAsync(
                    new GainPlayerExpCommand(request.UserId, expToGain)
                );
            }

            var currencyData = await _currencyDao.FindByUserIdAsync(request.UserId)
                               ?? throw new RequestException(
                                   ErrorStatusCode.NotFound,
                                   "PLAYER_CURRENCY_DATA_NOT_FOUND"
                               );

            var playerData = await _dataDao.FindByUserIdAsync(request.UserId)
                             ?? throw new RequestException(
                                 ErrorStatusCode.NotFound,
                                 "PLAYER_DATA_NOT_FOUND"
                             );

            return new ClaimStageRewardResult(
                Gold: currencyData.Gold,
                PaperPiece: currencyData.PaperPiece,
                Level: playerData.Level,
                Exp: playerData.Exp,
                IsReChallenge: isReChallenge
            );
        });
    }
}