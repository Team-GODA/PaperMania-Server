using Microsoft.AspNetCore.Mvc;
using Server.Api.Attribute;
using Server.Api.Dto.Response;
using Server.Api.Dto.Response.Currency;
using Server.Application.Port.Input.Currency;
using Server.Application.UseCase.Currency.Command;

namespace Server.Api.Controller.Currency
{
    [ApiLog("Currency")]
    [Route("api/v3/player/currency")]
    [ApiController]
    [SessionAuthorize]
    public class CurrencyController : BaseController
    {
        private readonly IGetCurrencyDataUseCase _getCurrencyDataUseCase;

        public CurrencyController(
            IGetCurrencyDataUseCase getCurrencyDataUseCase
            )
        {
            _getCurrencyDataUseCase = getCurrencyDataUseCase;
        }

        [HttpGet]
        public async Task<ActionResult<BaseResponse<GetCurrencyDataResponse>>> GetCurrencyData()
        {
            var result = await _getCurrencyDataUseCase.ExecuteAsync(new GetCurrencyDataCommand(
                UserId)
            );

            var response = new GetCurrencyDataResponse
            {
                ActionPoint = result.ActionPoint,
                Gold = result.Gold,
                PaperPiece = result.PaperPiece
            };
            
            return Ok(ApiResponse.Ok("플레이어 전체 재화 조회 성공", response));
        }
    }
}
