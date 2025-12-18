using Microsoft.AspNetCore.Mvc;
using Server.Api.Attribute;
using Server.Api.Dto.Request;
using Server.Api.Dto.Response;
using Server.Api.Dto.Response.Currency;
using Server.Application.Port.In.Currency;
using Server.Application.UseCase.Currency;
using Server.Application.UseCase.Currency.Command;

namespace Server.Api.Controller.Currency
{
    [Route("api/v3/player/currency/gold")]
    [ApiController]
    [SessionAuthorize]
    public class GoldController : BaseController
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
        public async Task<ActionResult<BaseResponse<GetPlayerGoldResponse>>> GetPlayerGold()
        {
            _logger.LogInformation($"플레이어 골드 조회 시도 : UserId : {UserId}");
            
            var gold = await _getGoldUseCase.ExecuteAsync(UserId);
            var response = new GetPlayerGoldResponse
            {
                CurrentGold = gold
            };
                
            _logger.LogInformation($"플레이어 골드 조회 성공 : UserId {UserId}");
            return Ok(ApiResponse.Ok("플레이어 골드 조회 성공", response));
        }

        [HttpPatch("gain")]
        public async Task<ActionResult<GetPlayerGoldResponse>> GainGold(
            [FromBody] GainGoldRequest request)
        {
            _logger.LogInformation($"플레이어 골드 추가 시도 : UserId : {UserId}");
            
            var gold = await _gainGoldUseCase.ExecuteAsync(new GainGoldCommand(
                UserId, request.Gold)
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
            _logger.LogInformation($"플레이어 골드 사용 시도 : UserID : {UserId}");

            var gold = await _spendGoldUseCase.ExecuteAsync(new SpendGoldCommand(
                UserId, request.Gold)
            );

            var response = new SpendGoldResponse
            {
                Gold = gold
            };

            return Ok(ApiResponse.Ok("플레이어 골드 사용 성공", response));
        }
    }
}
