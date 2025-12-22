using Microsoft.AspNetCore.Mvc;
using Server.Api.Attribute;
using Server.Api.Dto.Response;
using Server.Api.Dto.Response.Reward;
using Server.Application.Port.Input.Stage;
using Server.Application.UseCase.Stage.Command;

namespace Server.Api.Controller.Data
{
    [ApiLog("Reward")]
    [Route("api/v3/reward/stage")]
    [ApiController]
    public class RewardController : ControllerBase
    {
        private readonly IGetStageRewardUseCase _getStageRewardUseCase;
        private readonly ICheckStageClearedUseCase _checkStageClearedUseCase;

        public RewardController(
            IGetStageRewardUseCase getStageRewardUseCase,
            ICheckStageClearedUseCase checkStageClearedUseCase
            )
        {
            _getStageRewardUseCase = getStageRewardUseCase;
            _checkStageClearedUseCase = checkStageClearedUseCase;
        }
        
        /// <summary>
        /// 특정 스테이지의 보상 정보를 조회합니다.
        /// </summary>
        [HttpGet("{stageNum:int}/{stageSubNum:int}")]
        public ActionResult<BaseResponse<GetStageRewardResponse>> GetStageReward(
            [FromRoute] int stageNum,
            [FromRoute] int stageSubNum)
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
        // [SessionAuthorize]
        // [HttpPatch("stage")]
        // public async Task<ActionResult<BaseResponse<ClaimStageRewardResponse>>> ClaimStageReward(
        //     [FromBody] ClaimStageRewardRequest request)
        // {
        //     var sessionId = HttpContext.Items["SessionId"] as string;
        //     var userId = await _sessionService.FindUserIdBySessionIdAsync(sessionId!);
        //     
        //
        //     var stageData = new PlayerStageData
        //     {
        //         UserId = userId,
        //         StageNum = request.StageNum,
        //         StageSubNum = request.SubStageNum
        //     };
        //
        //     var stageReward = _rewardService.GetStageReward(request.StageNum, request.SubStageNum);
        //     await _rewardService.ClaimStageRewardByUserIdAsync(userId, stageReward!, stageData);
        //
        //     var response = new ClaimStageRewardResponse
        //     {
        //         Id = userId,
        //         StageReward = stageReward
        //     };
        //
        //     return Ok(ApiResponse.Ok("스테이지 보상 수령 성공", response));
        // }
    }
}
