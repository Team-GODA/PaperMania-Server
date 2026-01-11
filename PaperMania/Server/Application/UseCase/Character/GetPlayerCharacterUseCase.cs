using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.Input.Character;
using Server.Application.Port.Output.Persistence;
using Server.Application.UseCase.Character.Command;
using Server.Infrastructure.Persistence.Model;

namespace Server.Application.UseCase.Character;

public class GetPlayerCharacterUseCase : IGetPlayerCharacterUseCase
{
    private readonly ICharacterRepository _repository;

    public GetPlayerCharacterUseCase(ICharacterRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<PlayerCharacterData> ExecuteAsync(GetPlayerCharacterCommand request)
    {
        request.Validate();
        
        var data = await _repository.FindCharacter(request.UserId, request.CharacterId)
                   ?? throw new RequestException(
                       ErrorStatusCode.NotFound,
                       "PLAYER_CHARACTER_DATA_NOT_FOUND");
        
        return data;
    }
}