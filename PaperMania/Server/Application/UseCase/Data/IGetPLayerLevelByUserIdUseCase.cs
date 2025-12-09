namespace Server.Application.UseCase.Data;

public interface IGetPLayerLevelByUserIdUseCase
{
    Task<int> ExecuteAsync(int userId);
}