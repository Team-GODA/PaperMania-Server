using System.Net;
using Microsoft.AspNetCore.Mvc;
using Server.Api.Attribute;
using Server.Api.Dto.Request;
using Server.Api.Dto.Response;
using Server.Api.Dto.Response.Currency;
using Server.Application.Exceptions;
using Server.Application.Port;
using Server.Application.Port.Out.Service;
using Server.Application.UseCase.Currency;
using Server.Application.UseCase.Currency.Command;

namespace Server.Api.Controller
{
    [Route("api/v3/player/currency/action-point")]
    [ApiController]
    [SessionAuthorize]
    public class ApController : ControllerBase
    {
        private readonly GetActionPointUseCase _getActionPointUseCase;
        private readonly UpdateMaxActionPointUseCase _updateMaxActionPointUseCase;
        private readonly SpendActionPointUseCase _spendActionPointUseCase;
        
        private readonly ISessionService _sessionService;
        private readonly ILogger<CurrencyController> _logger;

        public ApController(
            GetActionPointUseCase getActionPointUseCase,
            UpdateMaxActionPointUseCase updateMaxActionPointUseCase,
            SpendActionPointUseCase spendActionPointUseCase,
            ISessionService sessionService, 
            ILogger<CurrencyController> logger
        )
        {
            _getActionPointUseCase = getActionPointUseCase;
            _updateMaxActionPointUseCase = updateMaxActionPointUseCase;
            _spendActionPointUseCase = spendActionPointUseCase;
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
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<GetPlayerActionPointResponse>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<BaseResponse<GetPlayerActionPointResponse>>> GetPlayerActionPointById()
        {
            var userId = GetUserId();
            
            _logger.LogInformation($"플레이어 AP 조회 시도 : UserId : {userId}");

            var result = await _getActionPointUseCase.ExecuteAsync(new GetActionPointCommand(
                userId)
            );

            var response = new GetPlayerActionPointResponse
            {
                CurrentActionPoint = result.ActionPoint
            };
                
            _logger.LogInformation($"플레이어 AP 조회 성공 : UserId : {userId}");
            return Ok(ApiResponse.Ok($"플레이어 AP 조회 성공 : UserId : {userId}", response));
        }
        
        /// <summary>
        /// 플레이어의 최대 행동력을 수정합니다.
        /// </summary>
        /// <param name="request">새로운 최대 행동력 정보</param>
        /// <returns>수정된 최대 행동력</returns>
        [HttpPatch("max")]
        [ProducesResponseType(typeof(BaseResponse<UpdatePlayerMaxActionPointResponse>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<BaseResponse<UpdatePlayerMaxActionPointResponse>>> UpdatePlayerMaxActionPoint(
            [FromBody] UpdatePlayerMaxActionPointRequest request)
        {
            var userId = GetUserId();
            
            _logger.LogInformation($"플레이어 최대 AP 갱신 시도");
            
            var result = await _updateMaxActionPointUseCase.ExecuteAsync(new UpdateMaxActionPointCommand(
                userId, request.NewMaxActionPoint)
            
            );
            var response = new UpdatePlayerMaxActionPointResponse
            {
                NewMaxActionPoint = result.MaxActionPoint
            };
                
            _logger.LogInformation($"플레이어 최대 AP 갱신 성공 : UserId : {userId}");
            return Ok(ApiResponse.Ok("플레이어 최대 AP 갱신 성공", response));
        }
        
        /// <summary>
        /// 플레이어의 행동력을 소모합니다.
        /// </summary>
        /// <param name="request">소모할 행동력 양</param>
        /// <returns>현재 행동력</returns>
        [HttpPatch]
        [ProducesResponseType(typeof(BaseResponse<UsePlayerActionPointResponse>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<BaseResponse<UsePlayerActionPointResponse>>> UsePlayerActionPoint(
            [FromBody] UsePlayerActionPointRequest request)
        {
            var userId = GetUserId();
            
            _logger.LogInformation($"플레이어 AP 사용 시도 : UserId : {userId}");
            
            var result = await _spendActionPointUseCase.ExecuteAsync(new UseActionPointCommand(
                userId, request.UsedActionPoint)
            
            );
        
            var response = new UsePlayerActionPointResponse
            {
                CurrentActionPoint = result.ActionPoint
            };
        
            _logger.LogInformation($"플레이어 AP 사용 성공 : UserId : {userId}");
            return Ok(ApiResponse.Ok("플레이어 AP 사용 성공", response));
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
