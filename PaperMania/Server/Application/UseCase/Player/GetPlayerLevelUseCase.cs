using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.Input.Player;
using Server.Application.Port.Output.Persistence;
using Server.Application.Port.Output.StaticData;
using Server.Application.UseCase.Player.Command;
using Server.Application.UseCase.Player.Result;
using Server.Domain.Entity;
using Server.Infrastructure.Cache;

namespace Server.Application.UseCase.Player;

public class GetPlayerLevelUseCase : IGetPlayerLevelUseCase
{
    private readonly IDataRepository _repository;
    private readonly ILevelDefinitionStore _store;

    public GetPlayerLevelUseCase(
        IDataRepository repository,
        ILevelDefinitionStore store
        )
    {
        _repository = repository;
        _store = store;
    }


    public async Task<GetPlayerLevelResult> ExecuteAsync(GetPlayerLevelCommand request)
    {
        var data = await _repository.FindByUserIdAsync(request.UserId)
            ?? throw new RequestException(
                ErrorStatusCode.NotFound,
                "PLAYER_NOT_FOUND",
                new { UserId = request.UserId });

        var levelDef = _store.GetLevelDefinition(data.Level)
                        ?? throw new RequestException(
                            ErrorStatusCode.NotFound,
                            "LEVEL_NOT_FOUND",
                            new { Level = data.Level });
            
        return new GetPlayerLevelResult(
            data.Level,
            data.Exp,
            levelDef.MaxExp
        );
    }
}