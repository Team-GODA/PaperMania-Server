using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port;

namespace Server.Application.UseCase.Data;

public class GetPlayerExpByUserIdService : IGetPlayerExpByUserIdUseCase
{
    private readonly IDataRepository _repository;
    private readonly IUnitOfWork  _unitOfWork;

    public GetPlayerExpByUserIdService(
        IDataRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Execute(int userId)
    {
        return await _unitOfWork.ExecuteAsync(async () =>
        {
            var data = await _repository.FindPlayerDataByUserIdAsync(userId);
            if (data == null)
                throw new RequestException(
                    ErrorStatusCode.NotFound,
                    "PLAYER_NOT_FOUND");
            
            return data.PlayerExp;
        });
    }
}