using Server.Application.UseCase.Data.Command;
using Server.Application.UseCase.Data.Result;

namespace Server.Application.UseCase.Data;

public interface IGetPlayerNameByUserIdUseCase
{
    Task<GetPlayerNameByUserIdResult> ExecuteAsync(GetPlayerNameByUserIdCommand request); 
}