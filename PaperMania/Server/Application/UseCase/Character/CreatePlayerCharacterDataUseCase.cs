using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.Input.Character;
using Server.Application.Port.Output.Infrastructure;
using Server.Application.Port.Output.Persistence;
using Server.Application.UseCase.Character.Command;
using Server.Application.UseCase.Character.Result;
using Server.Infrastructure.Persistence.Model;

namespace Server.Application.UseCase.Character;

public class CreatePlayerCharacterDataUseCase : ICreatePlayerCharacterDataUseCase
{
    private readonly ICharacterDao _dao;
    private readonly ITransactionScope _transactionScope;

    public CreatePlayerCharacterDataUseCase(
        ICharacterDao dao,
        ITransactionScope transactionScope
        )
    {
        _dao = dao; 
        _transactionScope = transactionScope;
    }
    
    public async Task<CreatePlayerCharacterDataResult> ExecuteAsync(CreatePlayerCharacterCommand request)
    {
        request.Validate();

        return await _transactionScope.ExecuteAsync(async () =>
        {
            var data = new PlayerCharacterData
            {
                UserId = request.UserId,
                CharacterId = request.CharacterId
            };

            var newCharacterData = await _dao.CreateAsync(data)
                                   ?? throw new RequestException(
                                       ErrorStatusCode.ServerError,
                                       "FAILED_CREATE_CHARACTER_DATA");

            return new CreatePlayerCharacterDataResult(
                newCharacterData.CharacterId
                );
        });
    }
}