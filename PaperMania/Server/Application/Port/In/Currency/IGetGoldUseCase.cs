namespace Server.Application.Port.In.Currency;

public interface IGetGoldUseCase
{
    Task<int> ExecuteAsync(int userId);
}