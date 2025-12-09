namespace Server.Application.UseCase.Data;

public interface IGetPlayerExpByUserIdUseCase
{
    Task<int> Execute(int userId);
}