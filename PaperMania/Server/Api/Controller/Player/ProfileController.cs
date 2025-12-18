using Microsoft.AspNetCore.Mvc;
using Server.Api.Attribute;
using Server.Api.Dto.Request;
using Server.Api.Dto.Response;
using Server.Api.Dto.Response.Data;
using Server.Application.UseCase.Player;
using Server.Application.UseCase.Player.Command;

namespace Server.Api.Controller.Player;

[ApiLog("Profile")]
[Route("api/v3/player/profile")]
[ApiController]
[SessionAuthorize]
public class ProfileController : BaseController
{
    private readonly GetPlayerNameUseCase _getPlayerNameUseCase;
    private readonly RenameUseCase _renameUseCase;

    public ProfileController(
        GetPlayerNameUseCase getPlayerNameUseCase,
        RenameUseCase renameUseCase
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
        var result = await _getPlayerNameUseCase.ExecuteAsync(
            new GetPlayerNameCommand(UserId)
        );

        var response = new GetPlayerNameResponse
        {
            Id = UserId,
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
        var result = await _renameUseCase.ExecuteAsync(
            new RenameCommand(UserId, request.NewName)
        );

        var response = new RenamePlayerNameResponse
        {
            Id = UserId,
            NewPlayerName = result.NewName
        };

        return Ok(ApiResponse.Ok("플레이어 이름 재설정 성공", response));
    }
}