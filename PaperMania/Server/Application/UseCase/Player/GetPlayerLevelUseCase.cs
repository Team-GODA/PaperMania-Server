using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.Input.Player;
using Server.Application.Port.Output.Cache;
using Server.Application.Port.Output.Persistence;
using Server.Application.Port.Output.StaticData;
using Server.Application.UseCase.Player.Command;
using Server.Application.UseCase.Player.Result;
using Server.Infrastructure.Cache;

namespace Server.Application.UseCase.Player;

public class GetPlayerLevelUseCase : IGetPlayerLevelUseCase
{
    private readonly IDataRepository _repository;
    private readonly ILevelDefinitionStore _store;
    private readonly ICacheAsideService _cache;

    public GetPlayerLevelUseCase(
        IDataRepository repository,
        ILevelDefinitionStore store,
        ICacheAsideService cache
        )
    {
        _repository = repository;
        _store = store;
        _cache = cache;
    }


    public async Task<GetPlayerLevelResult> ExecuteAsync(GetPlayerLevelCommand request, CancellationToken ct)
    {
        var player = await _cache.GetOrSetAsync(
            CacheKey.Profile.ByUserId(request.UserId),
            async (token) => await _repository.FindByUserIdAsync(request.UserId, token),
            TimeSpan.FromDays(30),
            ct
        );

        var levelDef = _store.GetLevelDefinition(player.Level)
                        ?? throw new RequestException(
                            ErrorStatusCode.NotFound,
                            "LEVEL_NOT_FOUND",
                            new { Level = player.Level });
            
        return new GetPlayerLevelResult(
            player.Level,
            player.Exp,
            levelDef.MaxExp
        );
    }
}