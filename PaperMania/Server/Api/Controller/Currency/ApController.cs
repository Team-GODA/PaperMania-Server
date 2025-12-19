using Microsoft.AspNetCore.Mvc;
using Server.Api.Attribute;
using Server.Api.Dto.Request;
using Server.Api.Dto.Response;
using Server.Api.Dto.Response.Currency;
using Server.Application.Port.Input.Currency;
using Server.Application.UseCase.Currency.Command;

namespace Server.Api.Controller.Currency
{
    [ApiLog("Currency-ActionPoint")]
    [Route("api/v3/player/currency/action-point")]
    [ApiController]
    [SessionAuthorize]
    public class ApController : BaseController
    {
        private readonly IGetActionPointUseCase _getActionPointUseCase;
        private readonly IUpdateMaxActionPointUseCase _updateMaxActionPointUseCase;
        private readonly ISpendActionPointUseCase _spendActionPointUseCase;

        public ApController(
            IGetActionPointUseCase getActionPointUseCase,
            IUpdateMaxActionPointUseCase updateMaxActionPointUseCase,
            ISpendActionPointUseCase spendActionPointUseCase
        )
        {
            _getActionPointUseCase = getActionPointUseCase;
            _updateMaxActionPointUseCase = updateMaxActionPointUseCase;
            _spendActionPointUseCase = spendActionPointUseCase;
        }
        
        /// <summary>
        /// 플레이어의 현재 행동력을 조회합니다.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<BaseResponse<GetPlayerActionPointResponse>>> GetPlayerActionPointById()
        {
            var result = await _getActionPointUseCase.ExecuteAsync(new GetActionPointCommand(
                UserId)
            );

            var response = new GetPlayerActionPointResponse
            {
                CurrentActionPoint = result.ActionPoint
            };
                
            return Ok(ApiResponse.Ok($"플레이어 AP 조회 성공 : UserId : {UserId}", response));
        }
        
        /// <summary>
        /// 플레이어의 최대 행동력을 수정합니다.
        /// </summary>
        [HttpPatch("max")]
        public async Task<ActionResult<BaseResponse<UpdatePlayerMaxActionPointResponse>>> UpdatePlayerMaxActionPoint(
            [FromBody] UpdatePlayerMaxActionPointRequest request)
        {
            var result = await _updateMaxActionPointUseCase.ExecuteAsync(new UpdateMaxActionPointCommand(
                UserId, request.NewMaxActionPoint)
            
            );
            var response = new UpdatePlayerMaxActionPointResponse
            {
                NewMaxActionPoint = result.MaxActionPoint
            };
                
            return Ok(ApiResponse.Ok("플레이어 최대 AP 갱신 성공", response));
        }
        
        /// <summary>
        /// 플레이어의 행동력을 소모합니다.
        /// </summary>
        [HttpPatch]
        public async Task<ActionResult<BaseResponse<UsePlayerActionPointResponse>>> UsePlayerActionPoint(
            [FromBody] UsePlayerActionPointRequest request)
        {
            var result = await _spendActionPointUseCase.ExecuteAsync(new SpendActionPointCommand(
                UserId, request.UsedActionPoint)
            
            );
        
            var response = new UsePlayerActionPointResponse
            {
                CurrentActionPoint = result.ActionPoint
            };
        
            return Ok(ApiResponse.Ok("플레이어 AP 사용 성공", response));
        }
    }
}
