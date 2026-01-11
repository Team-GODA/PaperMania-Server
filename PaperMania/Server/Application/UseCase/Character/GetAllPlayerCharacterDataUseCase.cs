using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.Input.Character;
using Server.Application.Port.Output.Persistence;
using Server.Infrastructure.Persistence.Model;

namespace Server.Application.UseCase.Character;

public class GetAllPlayerCharacterDataUseCase : IGetAllPlayerCharacterDataUseCase
{
    private readonly ICharacterRepository _repository;

    public GetAllPlayerCharacterDataUseCase(ICharacterRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<List<PlayerCharacterData>> ExecuteAsync(int userId)
    {
        if (userId <= 0)
            throw new RequestException(
                ErrorStatusCode.BadRequest,
                "INVALID_USER_ID"
                );

        var data = await _repository.FindAll(userId);

        return data.ToList();
    }
}