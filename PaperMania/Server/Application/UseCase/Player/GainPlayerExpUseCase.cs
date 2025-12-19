using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.Input.Player;
using Server.Application.Port.Output.Persistence;
using Server.Application.UseCase.Player.Command;
using Server.Application.UseCase.Player.Result;

namespace Server.Application.UseCase.Player;

public class GainPlayerExpUseCase : IGainPlayerExpUseCase
{
    private readonly IDataRepository _repository;

    public GainPlayerExpUseCase(
        IDataRepository repository
    )
    {
        _repository = repository;
    }
    
    public async Task<GainPlayerExpUseCaseResult> ExecuteAsync(GainPlayerExpUseCaseCommand request)
    {
        var data = await _repository.FindByUserIdAsync(request.UserId);
        if (data == null)
            throw new RequestException(
                ErrorStatusCode.NotFound,
                "PLAYER_DATA_NOT_FOUND");

        data.PlayerExp += request.Exp;

        while (true)
        {
            var levelData = await _repository.FindLevelDataAsync(data.PlayerLevel);
            if (levelData == null || data.PlayerExp < levelData.MaxExp)
                break;
            
            data.PlayerExp -= levelData.MaxExp;
            data.PlayerLevel++;
        }

        await _repository.UpdatePlayerLevelAsync(request.UserId, data.PlayerLevel, data.PlayerExp);
        return new GainPlayerExpUseCaseResult(
            Level:data.PlayerLevel,
            Exp:data.PlayerExp
            );
    }
}