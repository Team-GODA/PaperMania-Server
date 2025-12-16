using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port;
using Server.Application.UseCase.Data.Command;
using Server.Application.UseCase.Data.Result;

namespace Server.Application.UseCase.Data;

public class GetPlayerLevelByUserIdUseCase : IGetPlayerLevelByUserIdUseCase
{
    private readonly IDataRepository _repository;

    public GetPlayerLevelByUserIdUseCase(
        IDataRepository repository
        )
    {
        _repository = repository;
    }


    public async Task<GetPlayerLevelByUserIdResult> ExecuteAsync(GetPlayerLevelByUserIdCommand request)
    {
        var data = await _repository.FindByUserIdAsync(request.UserId);
        if (data == null)
            throw new RequestException(
                ErrorStatusCode.NotFound,
                "PLAYER_NOT_FOUND",
                new { UserId = request.UserId });

        return new GetPlayerLevelByUserIdResult(
            Level: data.PlayerLevel,
            Exp: data.PlayerExp
        );
    }
}