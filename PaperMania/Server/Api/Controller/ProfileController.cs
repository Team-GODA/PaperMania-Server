using System.Net;
using Microsoft.AspNetCore.Mvc;
using Server.Api.Attribute;
using Server.Api.Dto.Request;
using Server.Api.Dto.Response;
using Server.Api.Dto.Response.Data;
using Server.Application.UseCase.Player;
using Server.Application.UseCase.Player.Command;

namespace Server.Api.Controller;

[Route("api/v3/player/profile")]
[ApiController]
[SessionAuthorize]
public class ProfileController : ControllerBase
{
    private readonly GetPlayerNameByUserIdUseCase _getPlayerNameUseCase;
    private readonly RenameUseCase _renameUseCase;

    public ProfileController(
        GetPlayerNameByUserIdUseCase getPlayerNameUseCase,
        RenameUseCase renameUseCase
    )
    {
        _getPlayerNameUseCase = getPlayerNameUseCase;
        _renameUseCase = renameUseCase;
    }
        
    /// <summary>
    /// 현재 플레이어의 이름을 조회합니다.
    /// </summary>
    /// <returns>플레이어 이름 정보</returns>
    [HttpGet("name")]
    [ProducesResponseType(typeof(BaseResponse<GetPlayerNameResponse>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<BaseResponse<GetPlayerNameResponse>>> GetPlayerName()
    {
        var userId = (int)HttpContext.Items["UserId"]!;

        var result = await _getPlayerNameUseCase.ExecuteAsync(
            new GetPlayerNameByUserIdCommand(userId)
        );

        var response = new GetPlayerNameResponse
        {
            Id = userId,
            PlayerName = result.PlayerName
        };

        return Ok(ApiResponse.Ok("플레이어 이름 조회 성공", response));
    }
        
    /// <summary>
    /// 플레이어 이름을 변경합니다.
    /// </summary>
    /// <param name="request">변경할 새 플레이어 이름 정보</param>
    /// <returns>변경된 이름 반환</returns>
    [HttpPatch("name")]
    [ProducesResponseType(typeof(BaseResponse<RenamePlayerNameResponse>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<BaseResponse<RenamePlayerNameResponse>>> RenamePlayerName(
        [FromBody] RenamePlayerNameRequest request
    )
    {
        var userId = (int)HttpContext.Items["UserId"]!;

        var result = await _renameUseCase.ExecuteAsync(
            new RenameCommand(userId, request.NewName)
        );

        var response = new RenamePlayerNameResponse
        {
            Id = userId,
            NewPlayerName = result.NewName
        };

        return Ok(ApiResponse.Ok("플레이어 이름 재설정 성공", response));
    }
}