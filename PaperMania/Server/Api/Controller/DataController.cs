using System.Net;
using Microsoft.AspNetCore.Mvc;
using Server.Api.Dto.Request;
using Server.Application.Port;
using Asp.Versioning;
using Server.Api.Dto.Response;
using Server.Api.Dto.Response.Data;
using Server.Api.Filter;
using Server.Application.Exceptions;

namespace Server.Api.Controller
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(SessionValidationFilter))]
    public class DataController : ControllerBase
    {
        private readonly IDataService _dataService;
        private readonly ISessionService _sessionService;
        private readonly ILogger<DataController> _logger;

        public DataController(IDataService dataService, ISessionService sessionService ,ILogger<DataController> logger)
        {
            _dataService = dataService;
            _sessionService = sessionService;
            _logger = logger;
        }

        /// <summary>
        /// 플레이어 이름을 등록합니다.
        /// </summary>
        /// <param name="request">플레이어 이름 등록 요청 객체</param>
        /// <returns>등록 성공 여부에 대한 응답</returns>
        [HttpPost("player")]
        [ProducesResponseType(typeof(BaseResponse<AddPlayerDataResponse>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<BaseResponse<AddPlayerDataResponse>>> AddPlayerData([FromBody] AddPlayerDataRequest request)
        {
            _logger.LogInformation($"플레이어 데이터 등록 시도: PlayerName = {request.PlayerName}");
            var sessionId = HttpContext.Items["SessionId"] as string;

            try
            {
                var result = await _dataService.AddPlayerDataAsync(request.PlayerName, sessionId!);
                var response = new AddPlayerDataResponse
                {
                    PlayerName = result
                };

                _logger.LogInformation("플레이어 데이터 등록 성공: PlayerName = {PlayerName}", request.PlayerName);
                return Ok(ApiResponse.Ok("플레이어 데이터 등록 성공", response));
            }
            catch (PlayerNameExistException ex)
            {
                _logger.LogWarning(ex, "플레이어 이름 중복: {PlayerName}", request.PlayerName);
                return Ok(ApiResponse.Error<AddPlayerDataResponse>(ErrorStatusCode.Conflict, ex.Message));
            }
            catch (PlayerDataExistException ex)
            {
                _logger.LogWarning(ex, "이미 등록된 계정");
                return Ok(ApiResponse.Error<AddPlayerDataResponse>(ErrorStatusCode.Conflict, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "서버 오류 발생: 플레이어 데이터 등록 중 예외");
                return Ok(ApiResponse.Error<AddPlayerDataResponse>(ErrorStatusCode.ServerError, "서버 오류가 발생했습니다."));
            }
        }

        /// <summary>
        /// 현재 플레이어의 이름을 조회합니다.
        /// </summary>
        /// <returns>플레이어 이름 정보</returns>
        [HttpGet("name")]
        [ProducesResponseType(typeof(BaseResponse<GetPlayerNameResponse>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<BaseResponse<GetPlayerNameResponse>>> GetPlayerName()
        {
            var sessionId =  HttpContext.Items["SessionId"] as string;
            var userId = await _sessionService.GetUserIdBySessionIdAsync(sessionId!);

            _logger.LogInformation($"플레이어 이름 조회 시도: Id: {userId}");

            try
            {
                var playerName = await _dataService.GetPlayerNameByUserIdAsync(userId);

                var response = new GetPlayerNameResponse
                {
                    Id = userId,
                    PlayerName = playerName
                };

                _logger.LogInformation($"플레이어 이름 조회 성공: PlayerName: {playerName}");
                return Ok(ApiResponse.Ok("플레이어 이름 조회 성공", response));
            }
            catch (PlayerNotFoundException ex)
            {
                _logger.LogError(ex, "플레이어 데이터 조회 실패");
                return Ok(ApiResponse.Error<GetPlayerNameResponse>(ErrorStatusCode.NotFound, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "플레이어 이름 조회 중 오류 발생");
                return Ok(ApiResponse.Error<GetPlayerNameResponse>(ErrorStatusCode.ServerError, "서버 오류가 발생했습니다."));
            }
        }

        /// <summary>
        /// 플레이어 이름을 변경합니다.
        /// </summary>
        /// <param name="request">변경할 새 플레이어 이름 정보</param>
        /// <returns>변경된 이름 반환</returns>
        [HttpPatch("name")]
        [ProducesResponseType(typeof(BaseResponse<RenamePlayerNameResponse>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<BaseResponse<RenamePlayerNameResponse>>> RenamePlayerName([FromBody] RenamePlayerNameRequest request)
        {
            var sessionId =  HttpContext.Items["SessionId"] as string;
            var userId = await _sessionService.GetUserIdBySessionIdAsync(sessionId!);

            _logger.LogInformation($"플레이어 이름 재설정 시도: Id: {userId}");

            try
            {
                await _dataService.RenamePlayerNameAsync(userId, request.NewName);

                var response = new RenamePlayerNameResponse
                {
                    Id = userId,
                    NewPlayerName = request.NewName
                };

                _logger.LogInformation($"플레이어 이름 재설정 성공: Id: {userId}, NewName: {request.NewName}");
                return Ok(ApiResponse.Ok("플레이어 이름 재설정 성공", response));
            }
            catch (PlayerNameMissingException ex)
            {
                _logger.LogError(ex, "플레이어 이름 재설정 실패");
                return Ok(ApiResponse.Error<RenamePlayerNameResponse>(ErrorStatusCode.NotFound, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "플레이어 이름 재설정 중 오류 발생");
                return Ok(ApiResponse.Error<RenamePlayerNameResponse>(ErrorStatusCode.ServerError, "서버 오류가 발생했습니다."));
            }
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
            var userId = await _sessionService.GetUserIdBySessionIdAsync(sessionId!);

            _logger.LogInformation($"플레이어 레벨 조회 시도: Id: {userId}");

            try
            {
                var level = await _dataService.GetPlayerLevelByUserIdAsync(userId);
                var exp = await _dataService.GetPlayerExpByUserIdAsync(userId);

                var response = new GetPlayerLevelResponse
                {
                    Id = userId,
                    Level = level,
                    Exp = exp
                };

                _logger.LogInformation($"플레이어 레벨 조회 성공: PlayerLevel: {level}");
                return Ok(ApiResponse.Ok("플레이어 레벨 조회 성공", response));
            }
            catch (PlayerNotFoundException ex)
            {
                _logger.LogError(ex, "플레이어 데이터 조회 실패");
                return Ok(ApiResponse.Error<GetPlayerLevelResponse>(ErrorStatusCode.NotFound, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "플레이어 레벨 조회 중 오류 발생");
                return Ok(ApiResponse.Error<GetPlayerLevelResponse>(ErrorStatusCode.ServerError, "서버 오류가 발생했습니다."));
            }
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
            var userId = await _sessionService.GetUserIdBySessionIdAsync(sessionId!);

            _logger.LogInformation($"플레이어 레벨 갱신 시도: Id: {userId}");

            try
            {
                var data = await _dataService.UpdatePlayerLevelByExpAsync(userId, request.NewExp);

                var newLevel = data.PlayerLevel;
                var newExp = data.PlayerExp;

                var response = new UpdatePlayerLevelByExpResponse
                {
                    Id = userId,
                    NewLevel = newLevel,
                    NewExp = newExp
                };

                _logger.LogInformation($"플레이어 레벨 갱신 성공: Id: {userId}");
                return Ok(ApiResponse.Ok("플레이어 레벨 갱신 성공", response));
            }
            catch (PlayerNotFoundException ex)
            {
                _logger.LogError(ex, "플레이어 데이터 조회 실패");
                return Ok(ApiResponse.Error<UpdatePlayerLevelByExpResponse>(ErrorStatusCode.NotFound, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "플레이어 레벨 갱신 중 오류 발생");
                return Ok(ApiResponse.Error<UpdatePlayerLevelByExpResponse>(ErrorStatusCode.ServerError, "서버 오류가 발생했습니다."));
            }
        }
    }
}
