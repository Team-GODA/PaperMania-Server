using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port;
using Server.Domain.Entity;

namespace Server.Infrastructure.Service;

public class RewardService : IRewardService
{
    private readonly IRewardRepository _rewardRepository;
    private readonly IStageRepository _stageRepository;
    
    public RewardService(IRewardRepository rewardRepository, IStageRepository stageRepository)
    {
        _rewardRepository = rewardRepository;
        _stageRepository = stageRepository;
    }
    
    public StageReward GetStageReward(int stageNum, int stageSubNum)
    {
        var reward = GetStageDataOrException(stageNum, stageSubNum);
        return reward;
    }

    public async Task ClaimStageRewardByUserIdAsync(int? userId, StageReward reward, PlayerStageData data)
    {
        var stageReward = GetStageDataOrException(data.StageNum, data.SubStageNum);
        
        if (await _stageRepository.IsClearedStageAsync(data))
            stageReward.PaperPiece = 0;
        else
        {
            data.IsCleared = true;
            await _stageRepository.UpdateIsClearedAsync(data);
        }
        
        await _rewardRepository.ClaimStageRewardByUserIdAsync(userId, stageReward);
    }

    private StageReward GetStageDataOrException(int stageNum, int stageSubNum)
    {
        var reward = _rewardRepository.GetStageReward(stageNum, stageSubNum);
        if (reward == null)
            throw new RequestException(ErrorStatusCode.NotFound, "STAGE_NOT_FOUND",
                new { StageNum = stageNum, SubStageNum = stageSubNum });

        return reward;
    }
}