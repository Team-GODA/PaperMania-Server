using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.Input.Character;
using Server.Application.Port.Output.Persistence;
using Server.Infrastructure.Persistence.Model;

namespace Server.Application.UseCase.Character;

public class GetAllPlayerCharacterDataUseCase : IGetAllPlayerCharacterDataUseCase
{
    private readonly ICharacterDao _dao;

    public GetAllPlayerCharacterDataUseCase(ICharacterDao dao)
    {
        _dao = dao;
    }
    
    public async Task<List<PlayerCharacterData>> ExecuteAsync(int userId)
    {
        if (userId <= 0)
            throw new RequestException(
                ErrorStatusCode.BadRequest,
                "INVALID_USER_ID"
                );

        var data = await _dao.FindAll(userId);

        return data.ToList();
    }
}