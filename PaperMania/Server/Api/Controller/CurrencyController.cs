using Microsoft.AspNetCore.Mvc;
using Server.Api.Attribute;
using Server.Api.Dto.Response;
using Server.Api.Dto.Response.Currency;
using Server.Application.Port.In.Currency;
using Server.Application.Port.Out.Service;
using Server.Application.UseCase.Currency.Command;

namespace Server.Api.Controller
{
    [Route("api/v3/player/currency")]
    [ApiController]
    [SessionAuthorize]
    public class CurrencyController : BaseController
    {
        private readonly IGetCurrencyDataUseCase _getCurrencyDataUseCase;
        
        private readonly ISessionService _sessionService;
        private readonly ILogger<CurrencyController> _logger;

        public CurrencyController(
            IGetCurrencyDataUseCase getCurrencyDataUseCase,
            ISessionService sessionService, 
            ILogger<CurrencyController> logger
            )
        {
            _getCurrencyDataUseCase = getCurrencyDataUseCase;
            _sessionService = sessionService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<GetCurrencyDataResponse>> GetCurrencyData()
        {
            _logger.LogInformation($"플레이어 전체 재화 조회 시도 : UserId : {UserId}");
            
            var result = await _getCurrencyDataUseCase.ExecuteAsync(new GetCurrencyDataCommand(
                UserId)
            );

            var response = new GetCurrencyDataResponse
            {
                ActionPoint = result.ActionPoint,
                Gold = result.Gold,
                PaperPiece = result.PaperPiece
            };
            
            _logger.LogInformation($"플레이어 전체 재화 조회 성공 : UserId : {UserId}");
            return Ok(ApiResponse.Ok("플레이어 전체 재화 조회 성공", response));
        }
    }
}
