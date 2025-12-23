using Server.Application.UseCase.Character.Command;
using Server.Application.UseCase.Character.Result;

namespace Server.Application.Port.Input.Character;

public interface ICreatePlayerCharacterDataUseCase
{
    Task ExecuteAsync(CreatePlayerCharacterCommand request);
}