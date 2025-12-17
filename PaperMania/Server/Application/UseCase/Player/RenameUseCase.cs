using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port;
using Server.Application.Port.Out.Persistence;
using Server.Application.UseCase.Player.Command;
using Server.Application.UseCase.Player.Result;

namespace Server.Application.UseCase.Player;

public class RenameUseCase
{
    private readonly IDataRepository _repository;
    
    public RenameUseCase(IDataRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<RenameResult> ExecuteAsync(RenameCommand request)
    {
        if (string.IsNullOrEmpty(request.NewName))
            throw new RequestException(
                ErrorStatusCode.BadRequest,
                "NEW_NAME_EMPTY",
                new {UserId = request.UserId});

        var exist = await _repository.ExistsPlayerNameAsync(request.NewName);
        if (exist != null)
            throw new RequestException(
                ErrorStatusCode.Conflict,
                "PLAYER_NAME_EXIST");

        await _repository.RenamePlayerNameAsync(request.UserId, request.NewName);

        return new RenameResult(
            UserId: request.UserId,
            NewName: request.NewName
        );
    }
}