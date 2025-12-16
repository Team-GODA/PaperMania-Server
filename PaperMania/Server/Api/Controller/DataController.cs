using System.Net;
using Microsoft.AspNetCore.Mvc;
using Server.Api.Attribute;
using Server.Api.Dto.Request;
using Server.Application.Port;
using Server.Api.Dto.Response;
using Server.Api.Dto.Response.Data;
using Server.Application.UseCase.Data;
using Server.Application.UseCase.Data.Command;

namespace Server.Api.Controller
{
    [Route("api/v3/player/data")]
    [ApiController]
    [SessionAuthorize]
    public class DataController : ControllerBase
    {
        private readonly ICreatePlayerDataUseCase _createPlayerDataUseCase;
        private readonly IGetPlayerLevelByUserIdUseCase _getPlayerLevelByUserIdUseCase;
        private readonly IAddPlayerExpService _addPlayerExpService;
        private readonly ISessionService _sessionService;
        private readonly ILogger<DataController> _logger;

        public DataController(
            ICreatePlayerDataUseCase createPlayerDataUseCase,
            IGetPlayerLevelByUserIdUseCase getPlayerLevelByUserIdUseCase,
            IAddPlayerExpService addPlayerExpService,
            ISessionService sessionService,
            ILogger<DataController> logger
            )
        {
            _createPlayerDataUseCase = createPlayerDataUseCase;
            _getPlayerLevelByUserIdUseCase = getPlayerLevelByUserIdUseCase;
            _addPlayerExpService = addPlayerExpService;
            _sessionService = sessionService;
            _logger = logger;
        }

        /// <summary>
        /// 플레이어 데이터을 등록합니다.
        /// </summary>
        /// <param name="request">플레이어 이름 등록 요청 객체</param>
        /// <returns>등록 성공 여부에 대한 응답</returns>
        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse<AddPlayerDataResponse>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<BaseResponse<AddPlayerDataResponse>>> AddPlayerData([FromBody] AddPlayerDataRequest request)
        {
            _logger.LogInformation($"플레이어 데이터 등록 시도: PlayerName = {request.PlayerName}");
            var sessionId = HttpContext.Items["SessionId"] as string;

            var result =  await _createPlayerDataUseCase.ExecuteAsync(new AddPlayerDataCommand(
                request.PlayerName, sessionId!)
            );

            var response = new AddPlayerDataResponse
            {
                PlayerName = result.PlayerName
            };

            _logger.LogInformation("플레이어 데이터 등록 성공: PlayerName = {PlayerName}", request.PlayerName);
            return Ok(ApiResponse.Ok("플레이어 데이터 등록 성공", response));
        }

        /// <summary>
        /// 플레이어의 현재 레벨과 경험치를 조회합니다.
        /// </summary>
        /// <returns>레벨 및 경험치 정보</returns>
        [HttpGet("level")]
        [ProducesResponseType(typeof(BaseResponse<GetPlayerLevelResponse>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<BaseResponse<GetPlayerLevelResponse>>> GetPlayerLevel()
        {
            var sessionId = HttpContext.Items["SessionId"] as string;
            var userId = await _sessionService.FindUserIdBySessionIdAsync(sessionId!);

            _logger.LogInformation($"플레이어 레벨 조회 시도: UserId: {userId}");

            var result = await _getPlayerLevelByUserIdUseCase.ExecuteAsync(new GetPlayerLevelByUserIdCommand(
                userId)
            );

            var response = new GetPlayerLevelResponse
            {
                Level = result.Level,
                Exp = result.Exp
            };

            _logger.LogInformation($"플레이어 레벨 조회 성공: PlayerLevel: {result.Level}, PlayerExp: {result.Exp}");
            return Ok(ApiResponse.Ok("플레이어 레벨 조회 성공", response));
        }

        /// <summary>
        /// 플레이어의 경험치를 추가하고 레벨을 갱신합니다.
        /// </summary>
        /// <param name="request">추가할 경험치 정보</param>
        /// <returns>갱신된 레벨 및 경험치 정보</returns>
        [HttpPatch("level")]
        [ProducesResponseType(typeof(BaseResponse<UpdatePlayerLevelByExpResponse>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<BaseResponse<UpdatePlayerLevelByExpResponse>>> UpdatePlayerLevelByExp([FromBody] AddPlayerExpRequest request)
        {
            var sessionId =  HttpContext.Items["SessionId"] as string;
            var userId = await _sessionService.FindUserIdBySessionIdAsync(sessionId!);

            _logger.LogInformation($"플레이어 레벨 갱신 시도: UserId: {userId}");

            var result = await _addPlayerExpService.ExecuteAsync(new AddPlayerExpServiceCommand(
                userId, 
                request.NewExp)
            );

            var response = new UpdatePlayerLevelByExpResponse
            {
                Id = userId,
                NewLevel = result.Level,
                NewExp = result.Exp
            };

            _logger.LogInformation($"플레이어 레벨 갱신 성공: UserId: {userId}");
            return Ok(ApiResponse.Ok("플레이어 레벨 갱신 성공", response));
        }
    }
}
