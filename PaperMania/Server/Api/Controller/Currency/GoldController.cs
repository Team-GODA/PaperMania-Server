using Microsoft.AspNetCore.Mvc;
using Server.Api.Attribute;
using Server.Api.Dto.Request;
using Server.Api.Dto.Response;
using Server.Api.Dto.Response.Currency;
using Server.Application.Port.Input.Currency;
using Server.Application.UseCase.Currency.Command;

namespace Server.Api.Controller.Currency
{
    [ApiLog("Currency-Gold")]
    [Route("api/v3/player/currency/gold")]
    [ApiController]
    [SessionAuthorize]
    public class GoldController : BaseController
    {
        private readonly IGetGoldUseCase _getGoldUseCase;
        private readonly IGainGoldUseCase _gainGoldUseCase;
        private readonly ISpendGoldUseCase _spendGoldUseCase;

        public GoldController(
            IGetGoldUseCase getGoldUseCase,
            IGainGoldUseCase gainGoldUseCase,
            ISpendGoldUseCase spendGoldUseCase
        )
        {
            _getGoldUseCase = getGoldUseCase;
            _gainGoldUseCase = gainGoldUseCase;
            _spendGoldUseCase = spendGoldUseCase;
        }
        
        /// <summary>
        /// 플레이어의 골드를 조회합니다.
        /// </summary>
        /// <returns>현재 골드</returns>
        [HttpGet]
        public async Task<ActionResult<BaseResponse<BaseResponse<GetPlayerGoldResponse>>>> GetPlayerGold()
        {
            var result = await _getGoldUseCase.ExecuteAsync(UserId);
            var response = new GetPlayerGoldResponse
            {
                CurrentGold = result.Gold
            };
                
            return Ok(ApiResponse.Ok("플레이어 골드 조회 성공", response));
        }

        [HttpPatch("gain")]
        public async Task<ActionResult<BaseResponse<GetPlayerGoldResponse>>> GainGold(
            [FromBody] GainGoldRequest request)
        {
            var result = await _gainGoldUseCase.ExecuteAsync(new GainGoldCommand(
                UserId, request.Gold)
            );

            var response = new GainGoldResponse
            {
                Gold = result.Gold
            };
            
            return Ok(ApiResponse.Ok("플레이어 골드 추가 성공", response));
        }

        [HttpPatch("spend")]
        public async Task<ActionResult<BaseResponse<SpendGoldResponse>>> SpendGold(
            [FromBody] SpendGoldRequest request)
        {
            var result = await _spendGoldUseCase.ExecuteAsync(new SpendGoldCommand(
                UserId, request.Gold)
            );

            var response = new SpendGoldResponse
            {
                Gold = result.Gold
            };

            return Ok(ApiResponse.Ok("플레이어 골드 사용 성공", response));
        }
    }
}
