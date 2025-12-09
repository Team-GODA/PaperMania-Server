using System.Net;
using Microsoft.AspNetCore.Mvc;
using Server.Api.Dto.Request;
using Server.Api.Dto.Response;
using Server.Api.Dto.Response.Currency;
using Server.Api.Filter;
using Server.Application.Port;

namespace Server.Api.Controller
{
    [Route("api/v3/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(SessionValidationFilter))]
    public class CurrencyController : ControllerBase
    {
        private readonly ICurrencyService _currencyService;
        private readonly ISessionService _sessionService;
        private readonly ILogger<CurrencyController> _logger;

        public CurrencyController(ICurrencyService currencyService,ISessionService sessionService, ILogger<CurrencyController> logger)
        {
            _currencyService = currencyService;
            _sessionService = sessionService;
            _logger = logger;
        }
        
        /// <summary>
        /// 플레이어의 현재 행동력을 조회합니다.
        /// </summary>
        /// <remarks>
        /// 세션을 기반으로 사용자 ID를 식별하고, 현재 행동력을 조회합니다.
        /// </remarks>
        /// <returns>현재 행동력 정보</returns>
        [HttpGet("action-point")]
        [ProducesResponseType(typeof(BaseResponse<GetPlayerActionPointResponse>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<BaseResponse<GetPlayerActionPointResponse>>> GetPlayerActionPointById()
        {
            var sessionId = HttpContext.Items["SessionId"] as string;
            var userId = await _sessionService.FindUserIdBySessionIdAsync(sessionId!);
            
            _logger.LogInformation($"플레이어 AP 조회 시도 : UserId : {userId}");
            
            var currentActionPoint = await _currencyService.GetPlayerActionPointAsync(userId);
            var response = new GetPlayerActionPointResponse
            {
                CurrentActionPoint = currentActionPoint
            };                         
                
            _logger.LogInformation($"플레이어 AP 조회 성공 : UserId : {userId}");
            return Ok(ApiResponse.Ok($"플레이어 AP 조회 성공 : UserId : {userId}", response));
        }

        /// <summary>
        /// 플레이어의 최대 행동력을 수정합니다.
        /// </summary>
        /// <param name="request">새로운 최대 행동력 정보</param>
        /// <returns>수정된 최대 행동력</returns>
        [HttpPatch("action-point/max")]
        [ProducesResponseType(typeof(BaseResponse<UpdatePlayerMaxActionPointResponse>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<BaseResponse<UpdatePlayerMaxActionPointResponse>>> UpdatePlayerMaxActionPoint(
            [FromBody] UpdatePlayerMaxActionPointRequest request)
        {
            var sessionId = HttpContext.Items["SessionId"] as string;
            var userId = await _sessionService.FindUserIdBySessionIdAsync(sessionId!);
            
            _logger.LogInformation($"플레이어 최대 AP 갱신 시도");
            
            var newMaxActionPoint = await _currencyService.UpdatePlayerMaxActionPoint(userId, request.NewMaxActionPoint);
            var response = new UpdatePlayerMaxActionPointResponse
            {
                NewMaxActionPoint = newMaxActionPoint
            };
                
            _logger.LogInformation($"플레이어 최대 AP 갱신 성공 : UserId : {userId}");
            return Ok(ApiResponse.Ok("플레이어 최대 AP 갱신 성공", response));
        }

        /// <summary>
        /// 플레이어의 행동력을 소모합니다.
        /// </summary>
        /// <param name="request">소모할 행동력 양</param>
        /// <returns>현재 행동력</returns>
        [HttpPatch("action-point")]
        [ProducesResponseType(typeof(BaseResponse<UsePlayerActionPointResponse>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<BaseResponse<UsePlayerActionPointResponse>>> UsePlayerActionPoint(
            [FromBody] UsePlayerActionPointRequest request)
        {
            var sessionId = HttpContext.Items["SessionId"] as string;
            var userId = await _sessionService.FindUserIdBySessionIdAsync(sessionId!);
            
            _logger.LogInformation($"플레이어 AP 사용 시도 : UserId : {userId}");
            
            await _currencyService.UsePlayerActionPointAsync(userId, request.UsedActionPoint);
            var currentActionPoint = await _currencyService.GetPlayerActionPointAsync(userId);

            var response = new UsePlayerActionPointResponse
            {
                CurrentActionPoint = currentActionPoint
            };

            _logger.LogInformation($"플레이어 AP 사용 성공 : UserId : {userId}");
            return Ok(ApiResponse.Ok("플레이어 AP 사용 성공", response));
        }

        /// <summary>
        /// 플레이어의 골드를 조회합니다.
        /// </summary>
        /// <returns>현재 골드</returns>
        [HttpGet("gold")]
        [ProducesResponseType(typeof(BaseResponse<GetPlayerGoldResponse>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<BaseResponse<GetPlayerGoldResponse>>> GetPlayerGold()
        {
            var sessionId = HttpContext.Items["SessionId"] as string;
            var userId = await _sessionService.FindUserIdBySessionIdAsync(sessionId!);
            
            _logger.LogInformation($"플레이어 골드 조회 시도 : UserId : {userId}");
            
            var gold = await _currencyService.GetPlayerGoldAsync(userId);
            var response = new GetPlayerGoldResponse
            {
                CurrentGold = gold
            };
                
            _logger.LogInformation($"플레이어 골드 조회 성공 : UserId {userId}");
            return Ok(ApiResponse.Ok("플레이어 골드 조회 성공", response));
        }

        /// <summary>
        /// 플레이어의 골드를 수정합니다. (양수: 추가, 음수: 차감)
        /// </summary>
        /// <param name="request">변경할 골드 양</param>
        /// <returns>변경 후 현재 골드</returns>
        [HttpPatch("gold")]
        [ProducesResponseType(typeof(BaseResponse<ModifyGoldResponse>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<BaseResponse<ModifyGoldResponse>>> UpdatePlayerGold(
            [FromBody] ModifyGoldRequest request)
        {
            var sessionId = HttpContext.Items["SessionId"] as string;
            var userId = await _sessionService.FindUserIdBySessionIdAsync(sessionId!);

            _logger.LogInformation($"플레이어 골드 갱신 시도: UserId={userId}, Amount={request.Amount}");

            await _currencyService.ModifyPlayerGoldAsync(userId, request.Amount);
                
            var currentGold = await _currencyService.GetPlayerGoldAsync(userId);
            var response = new ModifyGoldResponse
            {
                CurrentGold = currentGold
            };
                
            _logger.LogInformation($"플레이어 골드 갱신 성공 : UserId {userId}");
            return Ok(ApiResponse.Ok("플레이어 골드 사용/추가 성공", response));
        }

        /// <summary>
        /// 플레이어의 종이 조각 수를 조회합니다.
        /// </summary>
        /// <returns>현재 종이 조각 개수</returns>
        [HttpGet("paper-piece")]
        [ProducesResponseType(typeof(BaseResponse<GetPlayerPaperPieceResponse>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<BaseResponse<GetPlayerPaperPieceResponse>>> GetPlayerPaperPiece()
        {
            var sessionId = HttpContext.Items["SessionId"] as string;
            var userId = await _sessionService.FindUserIdBySessionIdAsync(sessionId!);
            
            _logger.LogInformation($"플레이어 종이조각 조회 시도 : UserId : {userId}");

            var currentPaperPiece = await _currencyService.GetPlayerPaperPieceAsync(userId);
            var response = new GetPlayerPaperPieceResponse
            {
                CurrentPaperPieces = currentPaperPiece
            };

            _logger.LogInformation($"플레이어 종이조각 조회 성공 : UserId : {userId}");
            return Ok(ApiResponse.Ok("플레이어 종이조각 조회 성공", response));
        }

        /// <summary>
        /// 플레이어의 종이 조각을 수정합니다. (양수: 추가, 음수: 차감)
        /// </summary>
        /// <param name="request">변경할 종이 조각 수</param>
        /// <returns>변경 후 현재 종이 조각 수</returns>
        [HttpPatch("paper-piece")]
        [ProducesResponseType(typeof(BaseResponse<ModifyPaperPieceResponse>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<BaseResponse<ModifyPaperPieceResponse>>> UpdatePlayerPaperPiece(
            [FromBody] ModifyPaperPieceRequest request)
        {
            var sessionId = HttpContext.Items["SessionId"] as string;
            var userId = await _sessionService.FindUserIdBySessionIdAsync(sessionId!);

            _logger.LogInformation($"플레이어 종이 조각 갱신 시도 : UserId : {userId}");
            
            await _currencyService.ModifyPlayerPaperPieceAsync(userId, request.Amount);

            var currentPaperPiece = await _currencyService.GetPlayerPaperPieceAsync(userId);
            var response = new ModifyPaperPieceResponse
            {
                CurrentPaperPieces = currentPaperPiece
            };
                
            _logger.LogInformation($"플레이어 종이 조각 성공 : UserId {userId}");
            return Ok(ApiResponse.Ok("플레이어 종이 조각 성공",  response));
        }
    }
}
