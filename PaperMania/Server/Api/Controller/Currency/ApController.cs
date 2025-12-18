using System.Net;
using Microsoft.AspNetCore.Mvc;
using Server.Api.Attribute;
using Server.Api.Dto.Request;
using Server.Api.Dto.Response;
using Server.Api.Dto.Response.Currency;
using Server.Application.Port.In.Currency;
using Server.Application.UseCase.Currency.Command;

namespace Server.Api.Controller.Currency
{
    [Route("api/v3/player/currency/action-point")]
    [ApiController]
    [SessionAuthorize]
    public class ApController : BaseController
    {
        private readonly IGetActionPointUseCase _getActionPointUseCase;
        private readonly IUpdateMaxActionPointUseCase _updateMaxActionPointUseCase;
        private readonly ISpendActionPointUseCase _spendActionPointUseCase;
        private readonly ILogger<ApController> _logger;

        public ApController(
            IGetActionPointUseCase getActionPointUseCase,
            IUpdateMaxActionPointUseCase updateMaxActionPointUseCase,
            ISpendActionPointUseCase spendActionPointUseCase,
            ILogger<ApController> logger
        )
        {
            _getActionPointUseCase = getActionPointUseCase;
            _updateMaxActionPointUseCase = updateMaxActionPointUseCase;
            _spendActionPointUseCase = spendActionPointUseCase;
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
        public async Task<ActionResult<BaseResponse<GetPlayerActionPointResponse>>> GetPlayerActionPointById()
        {
            _logger.LogInformation($"플레이어 AP 조회 시도 : UserId : {UserId}");

            var result = await _getActionPointUseCase.ExecuteAsync(new GetActionPointCommand(
                UserId)
            );

            var response = new GetPlayerActionPointResponse
            {
                CurrentActionPoint = result.ActionPoint
            };
                
            _logger.LogInformation($"플레이어 AP 조회 성공 : UserId : {UserId}");
            return Ok(ApiResponse.Ok($"플레이어 AP 조회 성공 : UserId : {UserId}", response));
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
            _logger.LogInformation($"플레이어 최대 AP 갱신 시도");
            
            var result = await _updateMaxActionPointUseCase.ExecuteAsync(new UpdateMaxActionPointCommand(
                UserId, request.NewMaxActionPoint)
            
            );
            var response = new UpdatePlayerMaxActionPointResponse
            {
                NewMaxActionPoint = result.MaxActionPoint
            };
                
            _logger.LogInformation($"플레이어 최대 AP 갱신 성공 : UserId : {UserId}");
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
            _logger.LogInformation($"플레이어 AP 사용 시도 : UserId : {UserId}");
            
            var result = await _spendActionPointUseCase.ExecuteAsync(new SpendActionPointCommand(
                UserId, request.UsedActionPoint)
            
            );
        
            var response = new UsePlayerActionPointResponse
            {
                CurrentActionPoint = result.ActionPoint
            };
        
            _logger.LogInformation($"플레이어 AP 사용 성공 : UserId : {UserId}");
            return Ok(ApiResponse.Ok("플레이어 AP 사용 성공", response));
        }
    }
}
