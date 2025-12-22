using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.Input.Player;
using Server.Application.Port.Output.Persistence;
using Server.Application.UseCase.Player.Command;
using Server.Application.UseCase.Player.Result;

namespace Server.Application.UseCase.Player;

public class GainPlayerExpUseCase : IGainPlayerExpUseCase
{
    private readonly IDataDao _dao;

    public GainPlayerExpUseCase(
        IDataDao dao
    )
    {
        _dao = dao;
    }
    
    public async Task<GainPlayerExpUseCaseResult> ExecuteAsync(GainPlayerExpUseCaseCommand request)
    {
        var data = await _dao.FindByUserIdAsync(request.UserId);
        if (data == null)
            throw new RequestException(
                ErrorStatusCode.NotFound,
                "PLAYER_DATA_NOT_FOUND");

        data.PlayerExp += request.Exp;

        while (true)
        {
            var levelData = await _dao.FindLevelDataAsync(data.PlayerLevel);
            if (levelData == null || data.PlayerExp < levelData.MaxExp)
                break;
            
            data.PlayerExp -= levelData.MaxExp;
            data.PlayerLevel++;
        }

        await _dao.UpdatePlayerLevelAsync(request.UserId, data.PlayerLevel, data.PlayerExp);
        return new GainPlayerExpUseCaseResult(
            Level:data.PlayerLevel,
            Exp:data.PlayerExp
            );
    }
}