using Microsoft.AspNetCore.Mvc;
using Server.Api.Attribute;
using Server.Api.Dto.Request.Character;
using Server.Api.Dto.Response;
using Server.Api.Dto.Response.Character;
using Server.Application.Port.Input.Character;
using Server.Application.UseCase.Character.Command;

namespace Server.Api.Controller.Data;

[ApiLog("Character")]
[Route("api/v3/character")]
[ApiController]
[SessionAuthorize]
public class CharacterController : BaseController
{
    private readonly IGetPlayerCharacterUseCase _getPlayerCharacterUseCase;
    private readonly IGetAllPlayerCharacterDataUseCase _getAllPlayerCharacterDataUseCase;
    private readonly ICreatePlayerCharacterDataUseCase _createPlayerCharacterDataUseCase;

    public CharacterController(
        IGetPlayerCharacterUseCase getPlayerCharacterUseCase,
        IGetAllPlayerCharacterDataUseCase getAllPlayerCharacterDataUseCase,
        ICreatePlayerCharacterDataUseCase createPlayerCharacterDataUseCase
    )
    {
        _getPlayerCharacterUseCase = getPlayerCharacterUseCase;
        _getAllPlayerCharacterDataUseCase = getAllPlayerCharacterDataUseCase;
        _createPlayerCharacterDataUseCase = createPlayerCharacterDataUseCase;
    }
        
    /// <summary>
    /// 유저 보유 캐릭터 정보를 조회합니다.
    /// </summary>
    [HttpGet("{characterId:int}")]
    public async Task<ActionResult<BaseResponse<GetCharacterDataResponse>>> GetCharacterData(
        [FromRoute] int characterId)
    {
        var userId = GetUserId();

        var result = await _getPlayerCharacterUseCase.ExecuteAsync(new GetPlayerCharacterCommand(
            userId,
            characterId)
        );
            
        var response = new GetCharacterDataResponse
        {
            Character = result
        };

        return Ok(ApiResponse.Ok("플레이어 보유 캐릭터 데이터 조회 성공", response));
    }

    /// <summary>
    /// 유저의 보유 캐릭터를 전부 조회합니다.
    /// </summary>
    [HttpGet("all")]
    public async Task<ActionResult<BaseResponse<GetAllPlayerCharactersResponse>>> GetAllPlayerCharacters()
    {
        var userId = GetUserId();
        
        var result = await _getAllPlayerCharacterDataUseCase.ExecuteAsync(userId);

        var response = new GetAllPlayerCharactersResponse
        {
            Characters = result
        };
        
        return Ok(ApiResponse.Ok("플레이어 전체 보유 캐릭터 데이터 조회 성공", response));
    }

    /// <summary>
    /// 유저의 보유 캐릭터를 추가합니다.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<BaseResponse<EmptyResponse>>> AddPlayerCharacterData(
        [FromBody] AddPlayerCharacterRequest request)
    {
        var userId = GetUserId();

        await _createPlayerCharacterDataUseCase.ExecuteAsync(new CreatePlayerCharacterCommand(
            userId,
            request.CharacterId)
        );
            
        return Ok(ApiResponse.Ok<EmptyResponse>("플레이어 보유 캐릭터 추가 성공"));
    }
}