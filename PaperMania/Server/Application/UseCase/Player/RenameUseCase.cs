using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.Input.Player;
using Server.Application.Port.Output.Persistence;
using Server.Application.UseCase.Player.Command;
using Server.Application.UseCase.Player.Result;

namespace Server.Application.UseCase.Player;

public class RenameUseCase : IRenameUseCase
{
    private readonly IDataDao _dao;
    
    public RenameUseCase(IDataDao dao)
    {
        _dao = dao;
    }
    
    public async Task<RenameResult> ExecuteAsync(RenameCommand request)
    {
        request.Validate();

        var exist = await _dao.ExistsPlayerNameAsync(request.NewName);
        if (exist != null)
            throw new RequestException(
                ErrorStatusCode.Conflict,
                "PLAYER_NAME_EXIST");

        await _dao.RenamePlayerNameAsync(request.UserId, request.NewName);

        return new RenameResult(
            UserId: request.UserId,
            NewName: request.NewName
        );
    }
}