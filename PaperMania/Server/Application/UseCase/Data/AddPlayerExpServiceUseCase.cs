using Server.Application.UseCase.Data.Command;
using Server.Application.UseCase.Data.Result;

namespace Server.Application.UseCase.Data;

public interface IAddPlayerExpService
{
    Task<UpdatePlayerLevelByExpResult> ExecuteAsync(AddPlayerExpServiceCommand request);
}