using Microsoft.AspNetCore.Mvc;
using Server.Api.Attribute;
using Server.Api.Dto.Request.Character;
using Server.Api.Dto.Response;
using Server.Api.Dto.Response.Character;
using Server.Application.Port.Input.Character;
using Server.Application.UseCase.Character.Command;

namespace Server.Api.Controller.Data;

[Route("api/v3/character")]
[ApiController]
[SessionAuthorize]
public class CharacterController : BaseController
{
    private readonly IGetPlayerCharacterUseCase _getPlayerCharacterUseCase;
    private readonly ICreatePlayerCharacterDataUseCase _createPlayerCharacterDataUseCase;

    public CharacterController(
        IGetPlayerCharacterUseCase getPlayerCharacterUseCase,
        ICreatePlayerCharacterDataUseCase createPlayerCharacterDataUseCase
    )
    {
        _getPlayerCharacterUseCase = getPlayerCharacterUseCase;
        _createPlayerCharacterDataUseCase = createPlayerCharacterDataUseCase;
    }
        
    /// <summary>
    /// 특정 캐릭터 정보를 조회합니다.
    /// </summary>
    [HttpGet("{characterId:int}")]
    public async Task<ActionResult<BaseResponse<GetCharacterDataRequest>>> GetCharacterData(
        [FromRoute] int characterId)
    {
        var userId = GetUserId();

        var result = await _getPlayerCharacterUseCase.ExecuteAsync(new GetPlayerCharacterCommand(
            userId,
            characterId)
        );
            
        var response = new GetCharacterDataRequest
        {
            Character = result
        };

        return Ok(ApiResponse.Ok("플레이어 보유 캐릭터 데이터 조회 성공", response));
    }

    /// <summary>
    /// 유저의 보유 캐릭터를 추가합니다.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<BaseResponse<AddPlayerCharacterResponse>>> AddPlayerCharacterData(
        [FromBody] AddPlayerCharacterRequest request)
    {
        var userId = GetUserId();

        var result = await _createPlayerCharacterDataUseCase.ExecuteAsync(new CreatePlayerCharacterCommand(
            userId,
            request.CharacterId)
        );
        
        var response = new AddPlayerCharacterResponse
        {
            CharacterId = result.CharacterId
        };
            
        return Ok(ApiResponse.Ok("플레이어 보유 캐릭터 추가 성공", response));
    }
}