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
    private readonly IDataRepository _repository;

    public GetPlayerLevelUseCase(
        IDataRepository repository
        )
    {
        _repository = repository;
    }


    public async Task<GetPlayerLevelResult> ExecuteAsync(GetPlayerLevelCommand request)
    {
        var data = await _repository.FindByUserIdAsync(request.UserId)
            ?? throw new RequestException(
                ErrorStatusCode.NotFound,
                "PLAYER_NOT_FOUND",
                new { UserId = request.UserId });
            
        var gameState = new PlayerGameState
        {
            Level = data.PlayerLevel,
            Exp = data.PlayerExp
        };

        return new GetPlayerLevelResult(
            Level: gameState.Level,
            Exp: gameState.Exp
        );
    }
}