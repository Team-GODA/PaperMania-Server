using System.Net;
using Microsoft.AspNetCore.Mvc;
using Server.Api.Attribute;
using Server.Api.Dto.Request;
using Server.Api.Dto.Response;
using Server.Api.Dto.Response.Currency;
using Server.Application.Exceptions;
using Server.Application.Port.In.Currency;
using Server.Application.UseCase.Currency;
using Server.Application.UseCase.Currency.Command;

namespace Server.Api.Controller
{
    [Route("api/v3/player/currency/gold")]
    [ApiController]
    [SessionAuthorize]
    public class GoldController : ControllerBase
    {
        private readonly GetGoldUseCase _getGoldUseCase;
        private readonly IGainGoldUseCase _gainGoldUseCase;
        private readonly ISpendGoldUseCase _spendGoldUseCase;
        private readonly ILogger<GoldController> _logger;

        public GoldController(
            GetGoldUseCase getGoldUseCase,
            IGainGoldUseCase gainGoldUseCase,
            ISpendGoldUseCase spendGoldUseCase,
            ILogger<GoldController> logger
        )
        {
            _getGoldUseCase = getGoldUseCase;
            _gainGoldUseCase = gainGoldUseCase;
            _spendGoldUseCase = spendGoldUseCase;
            _logger = logger;
        }
        
        /// <summary>
        /// 플레이어의 골드를 조회합니다.
        /// </summary>
        /// <returns>현재 골드</returns>
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<GetPlayerGoldResponse>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<BaseResponse<GetPlayerGoldResponse>>> GetPlayerGold()
        {
            var userId = GetUserId();
            
            _logger.LogInformation($"플레이어 골드 조회 시도 : UserId : {userId}");
            
            var gold = await _getGoldUseCase.ExecuteAsync(userId);
            var response = new GetPlayerGoldResponse
            {
                CurrentGold = gold
            };
                
            _logger.LogInformation($"플레이어 골드 조회 성공 : UserId {userId}");
            return Ok(ApiResponse.Ok("플레이어 골드 조회 성공", response));
        }

        [HttpPatch("gain")]
        public async Task<ActionResult<GetPlayerGoldResponse>> GainGold(
            [FromBody] GainGoldRequest request)
        {
            var userId = GetUserId();
            
            _logger.LogInformation($"플레이어 골드 추가 시도 : UserId : {userId}");
            
            var gold = await _gainGoldUseCase.ExecuteAsync(new GainGoldCommand(
                userId, request.Gold)
            );

            var response = new GainGoldResponse
            {
                Gold = gold
            };
            
            return Ok(ApiResponse.Ok("플레이어 골드 추가 성공", response));
        }

        [HttpPatch("spend")]
        public async Task<ActionResult<SpendGoldResponse>> SpendGold(
            [FromBody] SpendGoldRequest request)
        {
            var userId = GetUserId();
            
            _logger.LogInformation($"플레이어 골드 사용 시도 : UserID : {userId}");

            var gold = await _spendGoldUseCase.ExecuteAsync(new SpendGoldCommand(
                userId, request.Gold)
            );

            var response = new SpendGoldResponse
            {
                Gold = gold
            };

            return Ok(ApiResponse.Ok("플레이어 골드 사용 성공", response));
        }
        
        private int GetUserId()
        {
            if (!HttpContext.Items.TryGetValue("UserId", out var userIdObj)
                || userIdObj is not int userId)
            {
                throw new RequestException(
                    ErrorStatusCode.Unauthorized,
                    "INVALID_SESSION");
            }
            
            return userId;
        }
    }
}
