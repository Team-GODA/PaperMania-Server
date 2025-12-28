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

public class CharacterController : BaseController
{
    private readonly IGetPlayerCharacterUseCase _getPlayerCharacterUseCase;
    private readonly IGetAllPlayerCharacterDataUseCase _getAllPlayerCharacterDataUseCase;
    private readonly ICreatePlayerCharacterDataUseCase _createPlayerCharacterDataUseCase;
    private readonly IGetAllCharacterDataUseCase _getAllCharacterDataUseCase;

    public CharacterController(
        IGetPlayerCharacterUseCase getPlayerCharacterUseCase,
        IGetAllPlayerCharacterDataUseCase getAllPlayerCharacterDataUseCase,
        ICreatePlayerCharacterDataUseCase createPlayerCharacterDataUseCase,
        IGetAllCharacterDataUseCase getAllCharacterDataUseCase
    )
    {
        _getPlayerCharacterUseCase = getPlayerCharacterUseCase;
        _getAllPlayerCharacterDataUseCase = getAllPlayerCharacterDataUseCase;
        _createPlayerCharacterDataUseCase = createPlayerCharacterDataUseCase;
        _getAllCharacterDataUseCase = getAllCharacterDataUseCase;
    }

    /// <summary>
    /// 전체 캐릭터 정보를 조회합니다.
    /// </summary>
    [SessionAuthorize]
    [HttpGet("all")]
    public ActionResult<BaseResponse<GetAllCharacterResponse>> GetAllCharacters()
    {
        var result = _getAllCharacterDataUseCase.Execute();

        var response = new GetAllCharacterResponse
        {
            Characters = result
        };
        
        return Ok(ApiResponse.Ok("캐릭터 전체 정보 조회", response));
    }
    
    /// <summary>
    /// 유저 보유 캐릭터 정보를 조회합니다.
    /// </summary>
    [SessionAuthorize]
    [HttpGet("player/{characterId:int}")]
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
    [HttpGet("player/all")]
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