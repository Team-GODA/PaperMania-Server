using System.Net;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Server.Api.Dto.Request;
using Server.Api.Dto.Response;
using Server.Api.Dto.Response.Currency;
using Server.Api.Filter;
using Server.Application.Port;

namespace Server.Api.Controller
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
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
        [ProducesResponseType(typeof(GetPlayerActionPointResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<GetPlayerActionPointResponse>> GetPlayerActionPointById()
        {
            var sessionId = HttpContext.Items["SessionId"] as string;
            var userId = await _sessionService.GetUserIdBySessionIdAsync(sessionId!);
            
            _logger.LogInformation($"플레이어 AP 조회 시도 : Id : {userId}");
            
            try
            {
                if (string.IsNullOrEmpty(sessionId))
                {
                    _logger.LogWarning("플레이어 AP 조회 실패: 세션 ID 없음");
                    return Ok(ApiResponse.Error<GetPlayerActionPointResponse>(1001, "SID가 없습니다."));
                }
                
                var currentActionPoint = await _currencyService.GetPlayerActionPointAsync(userId, sessionId!);
                var response = new GetPlayerActionPointResponse
                {
                    CurrentActionPoint = currentActionPoint
                };                         
                
                _logger.LogInformation($"플레이어 AP 조회 성공 : Id : {userId}");
                return Ok(ApiResponse.Ok($"플레이어 AP 조회 성공 : Id : {userId}", response));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "플레이어 AP 조회 중 오류 발생");
                return Ok(ApiResponse.Error<GetPlayerActionPointResponse>(5000, "서버 오류가 발생했습니다."));
            }
        }

        /// <summary>
        /// 플레이어의 최대 행동력을 수정합니다.
        /// </summary>
        /// <param name="request">새로운 최대 행동력 정보</param>
        /// <returns>수정된 최대 행동력</returns>
        [HttpPatch("action-point/max")]
        [ProducesResponseType(typeof(UpdatePlayerMaxActionPointResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<UpdatePlayerMaxActionPointResponse>> UpdatePlayerMaxActionPoint(
            [FromBody] UpdatePlayerMaxActionPointRequest request)
        {
            var sessionId = HttpContext.Items["SessionId"] as string;
            var userId = await _sessionService.GetUserIdBySessionIdAsync(sessionId!);
            
            _logger.LogInformation($"플레이어 최대 AP 갱신 시도");
            
            try
            {
                if (string.IsNullOrEmpty(sessionId))
                {
                    _logger.LogWarning("플레이어 최대 AP 갱신 실패: 세션 ID 없음");
                    return Ok(ApiResponse.Error<UpdatePlayerMaxActionPointResponse>(1001, "SID가 없습니다."));
                }
                
                var newMaxActionPoint = await _currencyService.UpdatePlayerMaxActionPoint(userId, request.NewMaxActionPoint, sessionId!);
                var response = new UpdatePlayerMaxActionPointResponse
                {
                    NewMaxActionPoint = newMaxActionPoint
                };
                
                _logger.LogInformation($"플레이어 최대 AP 갱신 성공 : Id : {userId}");
                return Ok(ApiResponse.Ok("플레이어 최대 AP 갱신 성공", response));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "플레이어 최대 AP 갱신 중 오류 발생");
                return Ok(ApiResponse.Error<UpdatePlayerMaxActionPointResponse>(5000, "서버 오류가 발생했습니다."));
            }
        }

        /// <summary>
        /// 플레이어의 행동력을 소모합니다.
        /// </summary>
        /// <param name="request">소모할 행동력 양</param>
        /// <returns>현재 행동력</returns>
        [HttpPatch("action-point")]
        [ProducesResponseType(typeof(UsePlayerActionPointResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<UsePlayerActionPointResponse>> UsePlayerActionPoint(
            [FromBody] UsePlayerActionPointRequest request)
        {
            var sessionId = HttpContext.Items["SessionId"] as string;
            var userId = await _sessionService.GetUserIdBySessionIdAsync(sessionId!);
            
            _logger.LogInformation($"플레이어 AP 사용 시도 : Id : {userId}");
            
            try
            {
                if (string.IsNullOrEmpty(sessionId))
                {
                    _logger.LogWarning("플레이어 AP 사용 실패: 세션 ID 없음");
                    return Ok(ApiResponse.Error<UsePlayerActionPointResponse>(1001, "SID가 없습니다."));
                }
                
                await _currencyService.UsePlayerActionPointAsync(userId, request.UsedActionPoint, sessionId!);
                var currentActionPoint = await _currencyService.GetPlayerActionPointAsync(userId, sessionId!);

                var response = new UsePlayerActionPointResponse
                {
                    CurrentActionPoint = currentActionPoint
                };

                _logger.LogInformation($"플레이어 AP 사용 성공 : Id : {userId}");
                return Ok(ApiResponse.Ok("플레이어 AP 사용 성공", response));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "플레이어 AP 사용 중 오류 발생");
                return Ok(ApiResponse.Error<UsePlayerActionPointResponse>(5000, "서버 오류가 발생했습니다."));
            }
        }

        /// <summary>
        /// 플레이어의 골드를 조회합니다.
        /// </summary>
        /// <returns>현재 골드</returns>
        [HttpGet("gold")]
        [ProducesResponseType(typeof(GetPlayerGoldReponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<GetPlayerGoldReponse>> GetPlayerGold()
        {
            var sessionId = HttpContext.Items["SessionId"] as string;
            var userId = await _sessionService.GetUserIdBySessionIdAsync(sessionId!);
            
            _logger.LogInformation($"플레이어 골드 조회 시도 : Id : {userId}");
            
            try
            {
                if (string.IsNullOrEmpty(sessionId))
                {
                    _logger.LogWarning("플레이어 골드 조회 실패: 세션 ID 없음");
                    return Ok(ApiResponse.Error<GetPlayerGoldReponse>(1001, "SID가 없습니다."));
                }
                
                var gold = await _currencyService.GetPlayerGoldAsync(userId, sessionId!);
                var response = new GetPlayerGoldReponse
                {
                    CurrentGold = gold
                };
                
                _logger.LogInformation($"플레이어 골드 조회 성공 : Id {userId}");
                return Ok(ApiResponse.Ok("플레이어 골드 조회 성공", response));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "플레이어 골드 조회 중 오류 발생");
                return Ok(ApiResponse.Error<GetPlayerGoldReponse>(5000, "서버 오류가 발생했습니다."));
            }
        }

        /// <summary>
        /// 플레이어의 골드를 수정합니다. (양수: 추가, 음수: 차감)
        /// </summary>
        /// <param name="request">변경할 골드 양</param>
        /// <returns>변경 후 현재 골드</returns>
        [HttpPatch("gold")]
        [ProducesResponseType(typeof(ModifyGoldResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<ModifyGoldResponse>> UpdatePlayerGold(
            [FromBody] ModifyGoldRequest request)
        {
            var sessionId = HttpContext.Items["SessionId"] as string;
            var userId = await _sessionService.GetUserIdBySessionIdAsync(sessionId!);

            _logger.LogInformation($"플레이어 골드 갱신 시도: Id={userId}, Amount={request.Amount}");

            try
            {
                if (string.IsNullOrEmpty(sessionId))
                {
                    _logger.LogWarning("플레이어 골드 갱신: 세션 ID 없음");
                    return Ok(ApiResponse.Error<ModifyGoldResponse>(1001, "SID가 없습니다."));
                }
                
                if (request.Amount >= 0)
                {
                    _logger.LogInformation($"플레이어 골드 추가 성공 :  {userId}, Amount : {request.Amount}");
                    await _currencyService.AddPlayerGoldAsync(userId, request.Amount, sessionId!);
                }
                else
                {
                    _logger.LogInformation($"플레이어 골드 사용 성공 : Id : {userId}, Amount : {request.Amount}");
                    await _currencyService.UsePlayerGoldAsync(userId, -request.Amount, sessionId!);
                }
                
                var currentGold = await _currencyService.GetPlayerGoldAsync(userId, sessionId!);
                var response = new ModifyGoldResponse
                {
                    CurrentGold = currentGold
                };
                
                _logger.LogInformation($"플레이어 골드 갱신 성공 : Id {userId}");
                return Ok(ApiResponse.Ok("플레이어 골드 사용/추가 성공", response));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "플레이어 골드 갱신 중 오류 발생");
                return Ok(ApiResponse.Error<ModifyGoldResponse>(5000, "서버 오류가 발생했습니다."));
            }
        }

        /// <summary>
        /// 플레이어의 종이 조각 수를 조회합니다.
        /// </summary>
        /// <returns>현재 종이 조각 개수</returns>
        [HttpGet("paper-piece")]
        [ProducesResponseType(typeof(GetPlayerPaperPieceReponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<GetPlayerPaperPieceReponse>> GetPlayerPaperPiece()
        {
            var sessionId = HttpContext.Items["SessionId"] as string;
            var userId = await _sessionService.GetUserIdBySessionIdAsync(sessionId!);
            
            _logger.LogInformation($"플레이어 종이조각 조회 시도 : Id : {userId}");

            try
            {
                if (string.IsNullOrEmpty(sessionId))
                {
                    _logger.LogWarning("플레이어 종이조각 조회 실패: 세션 ID 없음");
                    return Ok(ApiResponse.Error<GetPlayerPaperPieceReponse>(1001, "SID가 없습니다."));
                }
                
                var currentPaperPiece = await _currencyService.GetPlayerPaperPieceAsync(userId, sessionId!);
                var response = new GetPlayerPaperPieceReponse
                {
                    CurrentPaperPieces = currentPaperPiece
                };

                _logger.LogInformation($"플레이어 종이조각 조회 성공 : Id : {userId}");
                return Ok(ApiResponse.Ok("플레이어 종이조각 조회 성공", response));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "플레이어 종이조각 조회 중 오류 발생");
                return Ok(ApiResponse.Error<GetPlayerPaperPieceReponse>(5000, "서버 오류가 발생했습니다."));
            }
        }

        /// <summary>
        /// 플레이어의 종이 조각을 수정합니다. (양수: 추가, 음수: 차감)
        /// </summary>
        /// <param name="request">변경할 종이 조각 수</param>
        /// <returns>변경 후 현재 종이 조각 수</returns>
        [HttpPatch("paper-piece")]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<ModifyPaperPieceResponse>> UpdatePlayerPaperPiece(
            [FromBody] ModifyPaperPieceRequest request)
        {
            var sessionId = HttpContext.Items["SessionId"] as string;
            var userId = await _sessionService.GetUserIdBySessionIdAsync(sessionId!);

            _logger.LogInformation($"플레이어 종이 조각 갱신 시도 : Id : {userId}");
            
            try
            {
                if (string.IsNullOrEmpty(sessionId))
                {
                    _logger.LogWarning("플레이어 종이 조각 추가 실패: 세션 ID 없음");
                    return Ok(ApiResponse.Error<GetPlayerPaperPieceReponse>(1001, "SID가 없습니다."));
                }
                
                if (request.Amount >= 0)
                {
                    _logger.LogInformation($"플레이어 종이 조각 추가 성공 : Id : {userId}");
                    await _currencyService.AddPlayerPaperPieceAsync(userId, request.Amount, sessionId!);
                }
                else
                {
                    _logger.LogInformation($"플레이어 종이 조각 사용 성공 : Id : {userId}");
                    await _currencyService.UsePlayerPaperPieceAsync(userId, -request.Amount, sessionId!);
                }

                var currentPaperPiece = await _currencyService.GetPlayerPaperPieceAsync(userId, sessionId!);
                var response = new ModifyPaperPieceResponse
                {
                    CurrentPaperPieces = currentPaperPiece
                };
                
                _logger.LogInformation($"플레이어 종이 조각 성공 : Id {userId}");
                return Ok(ApiResponse.Ok("플레이어 종이 조각 성공",  response));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "플레이어 종이 조각 업데이트 중 오류 발생");
                return Ok(ApiResponse.Error<ModifyPaperPieceResponse>(5000, "서버 오류가 발생했습니다."));
            }
        }
    }
}
