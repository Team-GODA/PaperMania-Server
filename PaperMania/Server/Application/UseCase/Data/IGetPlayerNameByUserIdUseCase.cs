namespace Server.Application.UseCase.Data;

public interface IGetPlayerNameByUserIdUseCase
{
    Task<string> ExecuteAsync(int userId); 
}