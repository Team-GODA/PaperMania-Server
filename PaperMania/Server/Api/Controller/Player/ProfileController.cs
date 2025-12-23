using Microsoft.AspNetCore.Mvc;
using Server.Api.Attribute;
using Server.Api.Dto.Request;
using Server.Api.Dto.Request.Data;
using Server.Api.Dto.Response;
using Server.Api.Dto.Response.Data;
using Server.Application.Port.Input.Player;
using Server.Application.UseCase.Player.Command;

namespace Server.Api.Controller.Player;

[ApiLog("Profile")]
[Route("api/v3/player/profile")]
[ApiController]
[SessionAuthorize]
public class ProfileController : BaseController
{
    private readonly IGetPlayerNameUseCase _getPlayerNameUseCase;
    private readonly IRenameUseCase _renameUseCase;

    public ProfileController(
        IGetPlayerNameUseCase getPlayerNameUseCase,
        IRenameUseCase renameUseCase
    )
    {
        _getPlayerNameUseCase = getPlayerNameUseCase;
        _renameUseCase = renameUseCase;
    }
        
    /// <summary>
    /// 현재 플레이어의 이름을 조회합니다.
    /// </summary>
    [HttpGet("name")]
    public async Task<ActionResult<BaseResponse<GetPlayerNameResponse>>> GetPlayerName()
    {
        var userId = GetUserId();
        
        var result = await _getPlayerNameUseCase.ExecuteAsync(
            new GetPlayerNameCommand(userId)
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
    [HttpPatch("name")]
    public async Task<ActionResult<BaseResponse<RenamePlayerNameResponse>>> RenamePlayerName(
        [FromBody] RenamePlayerNameRequest request
    )
    {
        var userId = GetUserId();
        
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