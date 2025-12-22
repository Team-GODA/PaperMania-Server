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
    
    public async Task<GainPlayerExpUseCaseResult> ExecuteAsync(GainPlayerExpCommand request)
    {
        var data = await _dao.FindByUserIdAsync(request.UserId);
        if (data == null)
            throw new RequestException(
                ErrorStatusCode.NotFound,
                "PLAYER_DATA_NOT_FOUND");

        data.Exp += request.Exp;

        while (true)
        {
            var levelData = await _dao.FindLevelDataAsync(data.Level);
            if (levelData == null || data.Exp < levelData.MaxExp)
                break;
            
            data.Exp -= levelData.MaxExp;
            data.Level++;
        }

        await _dao.UpdatePlayerLevelAsync(request.UserId, data.Level, data.Exp);
        return new GainPlayerExpUseCaseResult(
            Level:data.Level,
            Exp:data.Exp
            );
    }
}