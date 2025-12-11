using Microsoft.AspNetCore.Mvc;
using Server.Api.Dto.Request;
using Server.Api.Dto.Response;
using Server.Api.Dto.Response.Character;
using Server.Api.Filter;
using Server.Application.Port;
using Server.Domain.Entity;

namespace Server.Api.Controller
{
    [Route("api/v3/[controller]")]
    [ServiceFilter(typeof(SessionValidationFilter))]
    [ApiController]
    public class CharacterController : ControllerBase
    {
        private readonly ICharacterService _characterService;
        private readonly ISessionService _sessionService;
        private readonly ILogger<CharacterController> _logger;

        public CharacterController(ICharacterService characterService, ILogger<CharacterController> logger,
            ISessionService sessionService)
        {
            _characterService = characterService;
            _sessionService = sessionService;
            _logger = logger;
        }
        
        /// <summary>
        /// 특정 캐릭터 정보를 조회합니다.
        /// </summary>
        /// <returns>캐릭터 정보</returns>
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<GetAllPlayerCharactersResponse>), 200)]
        public async Task<ActionResult<BaseResponse<GetAllPlayerCharactersResponse>>> GetAllPlayerCharacters()
        {
            var sessionId = HttpContext.Items["SessionId"] as string;
            var userId = await _sessionService.FindUserIdBySessionIdAsync(sessionId!);
            
            _logger.LogInformation($"플레이어 보유 캐릭터 데이터 조회 시도: ID: {userId}");

            var response = new GetAllPlayerCharactersResponse
            {
            };

            _logger.LogInformation($"플레이어 보유 캐릭터 데이터 조회 성공: ID: {userId}");
            return Ok(ApiResponse.Ok("플레이어 보유 캐릭터 데이터 조회 성공", response));
        }

        /// <summary>
        /// 유저의 보유 캐릭터를 추가합니다.
        /// </summary>
        /// <param name="request">추가할 캐릭터 정보</param>
        /// <returns>추가된 캐릭터 정보</returns>
        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse<AddPlayerCharacterResponse>), 200)]
        public async Task<ActionResult<BaseResponse<AddPlayerCharacterResponse>>> AddPlayerCharacter(
            [FromBody] AddPlayerCharacterRequest request)
        {
            _logger.LogInformation($"플레이어 보유 캐릭터 추가 시도: UserId: {request.Id}, CharacterId: {request.Data.CharacterId}");
            
            var data = new PlayerCharacterData
            {
                UserId = request.Id,
                Data = request.Data
            };

            var addedCharacter = await _characterService.AddPlayerCharacterDataByUserIdAsync(data);
            var response = new AddPlayerCharacterResponse
            {
                Id = addedCharacter.UserId,
                Data = addedCharacter.Data
            };
                
            _logger.LogInformation($"플레이어 보유 캐릭터 추가 성공: UserId: {request.Id}, CharacterId: {request.Data.CharacterId}");
            return Ok(ApiResponse.Ok("플레이어 보유 캐릭터 추가 성공", response));
        }
    }
}
