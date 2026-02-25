using FluentAssertions;
using Moq;
using Server.Application.Exceptions;
using Server.Application.Port.Output.Persistence;
using Server.Application.UseCase.Character;
using Server.Application.UseCase.Character.Command;
using Server.Infrastructure.Persistence.Model;

namespace Server.Tests.Application.Character;

public class GetPlayerCharacterUseCaseTests
{
    private readonly Mock<ICharacterRepository> _repoMock = new();

    private GetPlayerCharacterUseCase CreateUseCase()
        => new GetPlayerCharacterUseCase(_repoMock.Object);

    [Fact]
    public async Task ExecuteAsync_Should_Throw_When_PlayerCharacter_Not_Found()
    {
        var command = new GetPlayerCharacterCommand(1, 10);

        _repoMock.Setup(x => x.FindCharacter(1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PlayerCharacterData?)null);

        var useCase = CreateUseCase();

        Func<Task> act = () => useCase.ExecuteAsync(command, CancellationToken.None);

        var exception = await act.Should().ThrowAsync<RequestException>();
        exception.Which.StatusCode.Should().Be(Api.Dto.Response.ErrorStatusCode.NotFound);
        exception.Which.Message.Should().Be("PLAYER_CHARACTER_DATA_NOT_FOUND");
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_PlayerCharacterData_When_Found()
    {
        var command = new GetPlayerCharacterCommand(1, 10);

        var characterData = new PlayerCharacterData(
            UserId: 1,
            CharacterId: 10,
            CharacterLevel: 1,
            CharacterExp: 0,
            NormalSkillLevel: 1,
            UltimateSkillLevel: 1,
            SupportSkillLevel: 1
        );

        _repoMock.Setup(x => x.FindCharacter(1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(characterData);

        var useCase = CreateUseCase();

        var result = await useCase.ExecuteAsync(command, CancellationToken.None);

        result.Should().BeEquivalentTo(characterData);
    }
}