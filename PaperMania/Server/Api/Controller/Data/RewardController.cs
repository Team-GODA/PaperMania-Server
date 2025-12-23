using Microsoft.AspNetCore.Mvc;
using Server.Api.Attribute;
using Server.Api.Dto.Response;
using Server.Api.Dto.Response.Reward;
using Server.Application.Port.Input.Reward;
using Server.Application.UseCase.Reward.Command;

namespace Server.Api.Controller.Data
{
    [ApiLog("Reward")]
    [Route("api/v3/reward/stage")]
    [ApiController]
    public class RewardController : BaseController
    {
        private readonly IGetStageRewardUseCase _getStageRewardUseCase;
        private readonly IClaimStageRewardUseCase _claimStageRewardUseCase;

        public RewardController(
            IGetStageRewardUseCase getStageRewardUseCase,
            IClaimStageRewardUseCase claimStageRewardUseCase
            )
        {
            _getStageRewardUseCase = getStageRewardUseCase;
            _claimStageRewardUseCase = claimStageRewardUseCase;
        }
        
        /// <summary>
        /// 특정 스테이지의 보상 정보를 조회합니다.
        /// </summary>
        [HttpGet("{stageNum:int}/{stageSubNum:int}")]
        public ActionResult<BaseResponse<GetStageRewardResponse>> GetStageReward(
            [FromRoute] int stageNum,
            [FromRoute] int stageSubNum
            )
        {
            var reward = _getStageRewardUseCase.Execute(new GetStageRewardCommand(
                stageNum, stageSubNum)
            );
            
            var response = new GetStageRewardResponse
            {
                StageReward = reward
            };

            return Ok(ApiResponse.Ok("스테이지 보상 조회 성공", response));
        }

        /// <summary>
        /// 플레이어가 특정 스테이지 보상을 수령합니다.
        /// </summary>
        [SessionAuthorize]
        [HttpPost("{stageNum:int}/{stageSubNum:int}")]
        public async Task<ActionResult<BaseResponse<ClaimStageRewardResponse>>> ClaimStageReward(
            [FromRoute] int stageNum,
            [FromRoute] int stageSubNum
            )
        {
            var userId = GetUserId();
            
            var result = await _claimStageRewardUseCase.ExecuteAsync(new ClaimStageRewardCommand(
                userId,
                stageNum,
                stageSubNum)
            );

            var response = new ClaimStageRewardResponse
            {
                Gold = result.Gold,
                PaperPiece = result.PaperPiece,
                Level = result.Level,
                Exp = result.Exp,
                IsCleared = result.IsCleared
            };
        
            return Ok(ApiResponse.Ok("스테이지 보상 수령 성공", response));
        }
    }
}
