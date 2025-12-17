using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port;
using Server.Application.UseCase.Player.Command;
using Server.Application.UseCase.Player.Result;

namespace Server.Application.UseCase.Player;

public class AddPlayerExpUseCase
{
    private readonly IDataRepository _repository;

    public AddPlayerExpUseCase(
        IDataRepository repository
    )
    {
        _repository = repository;
    }
    
    public async Task<UpdatePlayerLevelByExpResult> ExecuteAsync(AddPlayerExpServiceCommand request)
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
        return new UpdatePlayerLevelByExpResult(
            Level:data.PlayerLevel,
            Exp:data.PlayerExp
            );
    }
}