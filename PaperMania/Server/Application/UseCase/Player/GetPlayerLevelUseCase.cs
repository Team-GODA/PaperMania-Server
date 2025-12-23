using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.Input.Player;
using Server.Application.Port.Output.Persistence;
using Server.Application.UseCase.Player.Command;
using Server.Application.UseCase.Player.Result;
using Server.Domain.Entity;
using Server.Infrastructure.Cache;

namespace Server.Application.UseCase.Player;

public class GetPlayerLevelUseCase : IGetPlayerLevelUseCase
{
    private readonly IDataDao _dao;

    public GetPlayerLevelUseCase(
        IDataDao dao
        )
    {
        _dao = dao;
    }


    public async Task<GetPlayerLevelResult> ExecuteAsync(GetPlayerLevelCommand request)
    {
        var data = await _dao.FindByUserIdAsync(request.UserId)
            ?? throw new RequestException(
                ErrorStatusCode.NotFound,
                "PLAYER_NOT_FOUND",
                new { UserId = request.UserId });
            
        var gameState = new PlayerGameState
        {
            Level = data.Level,
            Exp = data.Exp
        };

        return new GetPlayerLevelResult(
            Level: gameState.Level,
            Exp: gameState.Exp
        );
    }
}