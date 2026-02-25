using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.Input.Character;
using Server.Application.Port.Output.Persistence;
using Server.Application.Port.Output.StaticData;
using Server.Application.Port.Output.Transaction;
using Server.Application.UseCase.Character.Command;
using Server.Infrastructure.Persistence.Model;

namespace Server.Application.UseCase.Character;

public class CreatePlayerCharacterDataUseCase : ICreatePlayerCharacterDataUseCase
{
    private readonly ICharacterRepository _repository;
    private readonly ICharacterStore _store;
    private readonly ITransactionScope _transactionScope;

    public CreatePlayerCharacterDataUseCase(
        ICharacterRepository repository,
        ICharacterStore store,
        ITransactionScope transactionScope
        )
    {
        _repository = repository; 
        _store = store;
        _transactionScope = transactionScope;
    }
    
    public async Task ExecuteAsync(CreatePlayerCharacterCommand request, CancellationToken ct)
    {
        request.Validate();

        var character = _store.Get(request.CharacterId)
                        ?? throw new RequestException(
                            ErrorStatusCode.NotFound,
                            "CHARACTER_NOT_FOUND"
                            );
        
        await _transactionScope.ExecuteAsync(async (innerCt) =>
        {
            var data = new PlayerCharacterData(
                request.UserId,
                request.CharacterId, 
                1, 
                0,
                character.NormalSkillId == 0 ? 0 : 1,
                character.UltimateSkillId == 0 ? 0 : 1,
                character.SupportSkillId == 0 ? 0 : 1
            );

            await _repository.CreateAsync(data, innerCt);

            await _repository.CreatePieceData(new PlayerCharacterPieceData(data.UserId, data.CharacterId, 0),innerCt);
        }, ct);
    }
}