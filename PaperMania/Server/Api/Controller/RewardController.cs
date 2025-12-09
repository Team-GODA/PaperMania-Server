using Microsoft.AspNetCore.Mvc;
using Server.Api.Dto.Request;
using Server.Api.Dto.Response;
using Server.Api.Dto.Response.Reward;
using Server.Api.Filter;
using Server.Application.Port;
using Server.Domain.Entity;

namespace Server.Api.Controller
{
    [Route("api/v3/[controller]")]
    [ApiController]
    public class RewardController : ControllerBase
    {
        private readonly IRewardService _rewardService;
        private readonly ISessionService _sessionService;
        private readonly ILogger<RewardController> _logger;

        public RewardController(IRewardService rewardService, ISessionService sessionService,ILogger<RewardController> logger)
        {
            _rewardService = rewardService;
            _sessionService = sessionService;
            _logger = logger;
        }

        /// <summary>
        /// 특정 스테이지의 보상 정보를 조회합니다.
        /// </summary>
        /// <param name="stageNum">스테이지 번호</param>
        /// <param name="stageSubNum">서브 스테이지 번호</param>
        /// <returns>스테이지 보상 정보</returns>
        [HttpGet("stage/{stageNum:int}/{stageSubNum:int}")]
        [ProducesResponseType(typeof(BaseResponse<GetStageRewardResponse>), 200)]
        public ActionResult<BaseResponse<GetStageRewardResponse>> GetStageReward(
            [FromRoute] int stageNum,
            [FromRoute] int stageSubNum)
        {
            _logger.LogInformation($"스테이지 보상 조회 시도");

            var reward = _rewardService.GetStageReward(stageNum, stageSubNum);
            var response = new GetStageRewardResponse
            {
                StageReward = reward
            };

            _logger.LogInformation($"스테이지 보상 조회 성공 : StageNum = {stageNum}, StageSubNum = {stageSubNum}");
            return Ok(ApiResponse.Ok("스테이지 보상 조회 성공", response));
        }

        /// <summary>
        /// 플레이어가 특정 스테이지 보상을 수령합니다.
        /// </summary>
        /// <param name="request">수령할 스테이지 보상 정보</param>
        /// <returns>수령 결과 메시지와 보상 내역</returns>
        [HttpPatch("stage")]
        [ServiceFilter(typeof(SessionValidationFilter))]
        [ProducesResponseType(typeof(BaseResponse<ClaimStageRewardResponse>), 200)]
        public async Task<ActionResult<BaseResponse<ClaimStageRewardResponse>>> ClaimStageReward(
            [FromBody] ClaimStageRewardRequest request)
        {
            var sessionId = HttpContext.Items["SessionId"] as string;
            var userId = await _sessionService.FindUserIdBySessionIdAsync(sessionId!);
            
            _logger.LogInformation($"플레이어 스테이지 보상 수령 시도 : UserId : {userId}");

            var stageData = new PlayerStageData
            {
                UserId = userId,
                StageNum = request.StageNum,
                StageSubNum = request.SubStageNum
            };

            var stageReward = _rewardService.GetStageReward(request.StageNum, request.SubStageNum);
            await _rewardService.ClaimStageRewardByUserIdAsync(userId, stageReward!, stageData);

            var response = new ClaimStageRewardResponse
            {
                Id = userId,
                StageReward = stageReward
            };

            _logger.LogInformation($"플레이어 스테이지 보상 수령 성공 : UserId : {userId}");
            return Ok(ApiResponse.Ok("스테이지 보상 수령 성공", response));
        }
    }
}
