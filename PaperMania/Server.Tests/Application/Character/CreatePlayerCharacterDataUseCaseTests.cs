using FluentAssertions;
using Moq;
using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.Output.Persistence;
using Server.Application.Port.Output.StaticData;
using Server.Application.Port.Output.Transaction;
using Server.Application.UseCase.Character;
using Server.Application.UseCase.Character.Command;
using Server.Infrastructure.Persistence.Model;
using Server.Infrastructure.StaticData.Model;

namespace Server.Tests.Application.Character;

public class CreatePlayerCharacterDataUseCaseTests
{
    private readonly Mock<ICharacterRepository> _repoMock = new();
    private readonly Mock<ICharacterStore> _storeMock = new();
    private readonly Mock<ITransactionScope> _txMock = new();

    private CreatePlayerCharacterDataUseCase CreateUseCase()
        => new CreatePlayerCharacterDataUseCase(_repoMock.Object, _storeMock.Object, _txMock.Object);

    [Fact]
    public async Task ExecuteAsync_Should_Throw_When_Character_Not_Found()
    {
        var command = new CreatePlayerCharacterCommand(1, 10);

        _storeMock.Setup(x => x.Get(10)).Returns((CharacterData?)null);

        var useCase = CreateUseCase();

        Func<Task> act = () => useCase.ExecuteAsync(command, CancellationToken.None);

        var exception = await act.Should().ThrowAsync<RequestException>();
        exception.Which.StatusCode.Should().Be(ErrorStatusCode.NotFound);
        exception.Which.Message.Should().Be("CHARACTER_NOT_FOUND");
    }

    [Fact]
    public async Task ExecuteAsync_Should_Create_CharacterData_And_PieceData()
    {
        var command = new CreatePlayerCharacterCommand(1, 10);

        var character = new CharacterData(
            CharacterId: 10,
            CharacterName: "TestChar",
            Role: CharacterRole.Main,
            Rarity: CharacterRarity.Common,
            BaseHP: 100,
            BaseATK: 20,
            NormalSkillId: 1,
            UltimateSkillId: 2,
            SupportSkillId: 3
        );

        _storeMock.Setup(x => x.Get(10)).Returns(character);

        _txMock
            .Setup(x => x.ExecuteAsync(It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<CancellationToken, Task>, CancellationToken>((func, ct) => func(ct));

        var useCase = CreateUseCase();

        await useCase.ExecuteAsync(command, CancellationToken.None);

        _repoMock.Verify(x => x.CreateAsync(It.Is<PlayerCharacterData>(
            p => p.UserId == 1 && p.CharacterId == 10 && p.CharacterLevel == 1 && p.CharacterExp == 0
        ), It.IsAny<CancellationToken>()), Times.Once);

        _repoMock.Verify(x => x.CreatePieceData(It.Is<PlayerCharacterPieceData>(
            p => p.UserId == 1 && p.CharacterId == 10 && p.PieceAmount == 0
        ), It.IsAny<CancellationToken>()), Times.Once);
    }
}