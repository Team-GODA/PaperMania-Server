using Microsoft.AspNetCore.Mvc;
using Server.Api.Attribute;
using Server.Api.Dto.Request;
using Server.Api.Dto.Request.Data;
using Server.Api.Dto.Response;
using Server.Api.Dto.Response.Data;
using Server.Application.Port.Input.Player;
using Server.Application.UseCase.Player.Command;

namespace Server.Api.Controller.Player
{
    [ApiLog("Data")]
    [Route("api/v3/player/data")]
    [ApiController]
    [SessionAuthorize]
    public class DataController : BaseController
    {
        private readonly ICreatePlayerDataUseCase _createPlayerDataUseCase;
        private readonly IGetPlayerLevelUseCase _getPlayerLevelUseCase;
        private readonly IGainPlayerExpUseCase _gainPlayerExpUseCase;

        public DataController(
            ICreatePlayerDataUseCase createPlayerDataUseCase,
            IGetPlayerLevelUseCase getPlayerLevelUseCase,
            IGainPlayerExpUseCase gainPlayerExpUseCase
            )
        {
            _createPlayerDataUseCase = createPlayerDataUseCase;
            _getPlayerLevelUseCase = getPlayerLevelUseCase;
            _gainPlayerExpUseCase = gainPlayerExpUseCase;
        }

        /// <summary>
        /// 플레이어 데이터을 등록합니다.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<BaseResponse<AddPlayerDataResponse>>> AddPlayerData([FromBody] AddPlayerDataRequest request)
        {
            var sessionId = GetSessionId();
            
            var result =  await _createPlayerDataUseCase.ExecuteAsync(new AddPlayerDataCommand(
                request.PlayerName, sessionId)
            );

            var response = new AddPlayerDataResponse
            {
                PlayerName = result.PlayerName
            };

            return Ok(ApiResponse.Ok("플레이어 데이터 등록 성공", response));
        }

        /// <summary>
        /// 플레이어의 현재 레벨과 경험치를 조회합니다.
        /// </summary>
        [HttpGet("level")]
        public async Task<ActionResult<BaseResponse<GetPlayerLevelResponse>>> GetPlayerLevel()
        {
            var userId = GetUserId();
            
            var result = await _getPlayerLevelUseCase.ExecuteAsync(new GetPlayerLevelCommand(
                userId)
            );

            var response = new GetPlayerLevelResponse
            {
                Level = result.Level,
                Exp = result.Exp
            };

            return Ok(ApiResponse.Ok("플레이어 레벨 조회 성공", response));
        }

        /// <summary>
        /// 플레이어의 경험치를 추가하고 레벨을 갱신합니다.
        /// </summary>
        [HttpPatch("level")]
        public async Task<ActionResult<BaseResponse<UpdatePlayerLevelByExpResponse>>> UpdatePlayerLevelByExp([FromBody] AddPlayerExpRequest request)
        {
            var userId = GetUserId();
            
            var result = await _gainPlayerExpUseCase.ExecuteWithTransactionAsync(new GainPlayerExpCommand(
                userId, 
                request.NewExp)
            );

            var response = new UpdatePlayerLevelByExpResponse
            {
                Id = userId,
                NewLevel = result.Level,
                NewExp = result.Exp
            };

            return Ok(ApiResponse.Ok("플레이어 레벨 갱신 성공", response));
        }
    }
}
