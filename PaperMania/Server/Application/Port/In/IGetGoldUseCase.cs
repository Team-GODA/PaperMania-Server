namespace Server.Application.Port.In;

public interface IGetGoldUseCase
{
    Task<int> ExecuteAsync(int userId);
}