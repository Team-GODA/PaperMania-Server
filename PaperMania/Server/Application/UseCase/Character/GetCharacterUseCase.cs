using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.Input.Character;
using Server.Application.Port.Output.Persistence;
using Server.Application.UseCase.Character.Command;
using Server.Infrastructure.Persistence.Model;

namespace Server.Application.UseCase.Character;

public class GetCharacterUseCase : IGetCharacterUseCase
{
    private readonly ICharacterDao _dao;

    public GetCharacterUseCase(ICharacterDao dao)
    {
        _dao = dao;
    }
    
    public async Task<PlayerCharacterData> ExecuteAsync(GetCharacterCommand request)
    {
        request.Validate();
        
        var data = await _dao.FindCharacter(request.UserId, request.CharacterId)
                   ?? throw new RequestException(
                       ErrorStatusCode.NotFound,
                       "PLAYER_CHARACTER_DATA_NOT_FOUND");
        
        return data;
    }
}