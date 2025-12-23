using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.Input.Character;
using Server.Application.Port.Output.Infrastructure;
using Server.Application.Port.Output.Persistence;
using Server.Application.Port.Output.StaticData;
using Server.Application.UseCase.Character.Command;
using Server.Infrastructure.Persistence.Model;

namespace Server.Application.UseCase.Character;

public class CreatePlayerCharacterDataUseCase : ICreatePlayerCharacterDataUseCase
{
    private readonly ICharacterDao _dao;
    private readonly ICharacterStore _store;
    private readonly ITransactionScope _transactionScope;

    public CreatePlayerCharacterDataUseCase(
        ICharacterDao dao,
        ICharacterStore store,
        ITransactionScope transactionScope
        )
    {
        _dao = dao; 
        _store = store;
        _transactionScope = transactionScope;
    }
    
    public async Task ExecuteAsync(CreatePlayerCharacterCommand request)
    {
        request.Validate();

        var character = _store.Get(request.CharacterId)
                        ?? throw new RequestException(
                            ErrorStatusCode.NotFound,
                            "CHARACTER_NOT_FOUND"
                            );
        
        await _transactionScope.ExecuteAsync(async () =>
        {
            var data = new PlayerCharacterData
            {
                UserId = request.UserId,
                CharacterId = request.CharacterId,

                CharacterLevel = 1,
                CharacterExp = 0,

                NormalSkillLevel =
                    character.NormalSkillId == 0 ? 0 : 1,

                UltimateSkillLevel =
                    character.UltimateSkillId == 0 ? 0 : 1,

                SupportSkillLevel =
                    character.SupportSkillId == 0 ? 0 : 1
            };

            await _dao.CreateAsync(data);
            
            await _dao.CreatePieceData(new PlayerCharacterPieceData
            {
                UserId = data.UserId,
                CharacterId = data.CharacterId,
                PieceAmount = 0
            });
        });
    }
}