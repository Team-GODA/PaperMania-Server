using FluentAssertions;
using Moq;
using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.Output.Persistence;
using Server.Application.Port.Output.StaticData;
using Server.Application.UseCase.Player;
using Server.Application.UseCase.Player.Command;
using Server.Domain.Entity;
using Server.Infrastructure.StaticData.Model;

namespace Server.Tests.Application.Player;

public class GetPlayerLevelUseCaseTests
{
    private readonly Mock<IDataRepository> _repositoryMock = new();
    private readonly Mock<ILevelDefinitionStore> _levelStoreMock = new();

    private GetPlayerLevelUseCase CreateUseCase() =>
        new(_repositoryMock.Object, _levelStoreMock.Object);

    [Fact]
    public async Task ExecuteAsync_Should_Throw_When_Player_Not_Found()
    {
        _repositoryMock
            .Setup(x => x.FindByUserIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PlayerData?)null);

        var useCase = CreateUseCase();
        var command = new GetPlayerLevelCommand(1);

        Func<Task> act = () => useCase.ExecuteAsync(command, CancellationToken.None);

        var exception = await act.Should().ThrowAsync<RequestException>();
        exception.Which.StatusCode.Should().Be(ErrorStatusCode.NotFound);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_When_Level_Not_Found()
    {
        var gameData = PlayerData.Create(1, "Player");

        _repositoryMock
            .Setup(x => x.FindByUserIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(gameData);

        _levelStoreMock
            .Setup(x => x.GetLevelDefinition(It.IsAny<int>()))
            .Returns((LevelDefinition?)null);

        var useCase = CreateUseCase();
        var command = new GetPlayerLevelCommand(1);

        Func<Task> act = () => useCase.ExecuteAsync(command, CancellationToken.None);

        var exception = await act.Should().ThrowAsync<RequestException>();
        exception.Which.StatusCode.Should().Be(ErrorStatusCode.NotFound);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Level_Info_When_Success()
    {
        var gameData = PlayerData.Create(1, "Player");
        var levelDefinition = new LevelDefinition(1,100, 50);

        _repositoryMock
            .Setup(x => x.FindByUserIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(gameData);

        _levelStoreMock
            .Setup(x => x.GetLevelDefinition(1))
            .Returns(levelDefinition);

        var useCase = CreateUseCase();
        var command = new GetPlayerLevelCommand(1);

        var result = await useCase.ExecuteAsync(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Level.Should().Be(1);
        result.Exp.Should().Be(gameData.Exp);
        result.MaxExp.Should().Be(100);
    }
}