using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.In.Player;
using Server.Application.Port.Out.Persistence;
using Server.Application.UseCase.Player.Command;
using Server.Application.UseCase.Player.Result;
using Server.Domain.Entity;
using Server.Infrastructure.Cache;

namespace Server.Application.UseCase.Player;

public class GetPlayerLevelUseCase : IGetPlayerLevelUseCase
{
    private readonly IDataRepository _repository;
    private readonly CacheWrapper _cache;

    public GetPlayerLevelUseCase(
        IDataRepository repository,
        CacheWrapper cache
        )
    {
        _repository = repository;
        _cache = cache;       
    }


    public async Task<GetPlayerLevelResult> ExecuteAsync(GetPlayerLevelCommand request)
    {
        var gameState = await _cache.FetchAsync(
            CacheKey.Player.GameData(request.UserId),
            async () =>
            {
                var data = await _repository.FindByUserIdAsync(request.UserId);
                if (data == null) 
                    return null;

                return new PlayerGameState
                {
                    Level = data.PlayerLevel,
                    Exp = data.PlayerExp
                };
            },
            TimeSpan.FromDays(30)
        );
        
        if (gameState  == null)
            throw new RequestException(
                ErrorStatusCode.NotFound,
                "PLAYER_NOT_FOUND",
                new { UserId = request.UserId });

        return new GetPlayerLevelResult(
            Level: gameState.Level,
            Exp: gameState.Exp
        );
    }
}