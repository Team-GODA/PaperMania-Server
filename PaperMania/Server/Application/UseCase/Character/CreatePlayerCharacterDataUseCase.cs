using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.Input.Character;
using Server.Application.Port.Output.Persistence;
using Server.Application.Port.Output.StaticData;
using Server.Application.Port.Output.Transaction;
using Server.Application.UseCase.Character.Command;
using Server.Domain.Entity;

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

        var characterDef  = _store.Get(request.CharacterId)
                            ?? throw new RequestException(
                                ErrorStatusCode.NotFound,
                                "CHARACTER_NOT_FOUND"
                            );

        await _transactionScope.ExecuteAsync(async (innerCt) =>
        {
            var character  = PlayerCharacter.Create(request.UserId, request.CharacterId, characterDef.Role);

            await _repository.CreateAsync(character, innerCt);
            await _repository.CreatePieceData(character, innerCt);
        }, ct);
    }
}