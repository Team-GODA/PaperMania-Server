using FluentAssertions;
using Moq;
using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.Output.Persistence;
using Server.Application.Port.Output.StaticData;
using Server.Application.Port.Output.Transaction;
using Server.Application.UseCase.Player;
using Server.Application.UseCase.Player.Command;
using Server.Application.UseCase.Player.Result;
using Server.Domain.Entity;
using Server.Infrastructure.StaticData.Model;

namespace Server.Tests.Application.Player;

public class GainPlayerExpUseCaseTests
{
    private readonly Mock<IDataRepository> _dataRepositoryMock = new();
    private readonly Mock<ICurrencyRepository> _currencyRepositoryMock = new();
    private readonly Mock<ILevelDefinitionStore> _levelStoreMock = new();
    private readonly Mock<ITransactionScope> _transactionScopeMock = new();

    private GainPlayerExpUseCase CreateUseCase() => new(
        _dataRepositoryMock.Object,
        _currencyRepositoryMock.Object,
        _levelStoreMock.Object,
        _transactionScopeMock.Object
    );
    
    [Fact]
    public async Task ExecuteAsync_Should_Throw_When_Player_Not_Found()
    {
        _dataRepositoryMock
            .Setup(x => x.FindByUserIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GameData?)null);

        var useCase = CreateUseCase();
        var command = new GainPlayerExpCommand(1, 100);

        Func<Task> act = () => useCase.ExecuteAsync(command, CancellationToken.None);

        var exception = await act.Should().ThrowAsync<RequestException>();
        exception.Which.StatusCode.Should().Be(ErrorStatusCode.NotFound);
    }
    
    [Fact]
    public async Task ExecuteAsync_Should_Throw_When_Currency_Not_Found()
    {
        var gameData = GameData.Create(1, "Player");

        _dataRepositoryMock
            .Setup(x => x.FindByUserIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(gameData);

        _currencyRepositoryMock
            .Setup(x => x.FindByUserIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Currency?)null);

        var useCase = CreateUseCase();
        var command = new GainPlayerExpCommand(1, 100);

        Func<Task> act = () => useCase.ExecuteAsync(command, CancellationToken.None);

        var exception = await act.Should().ThrowAsync<RequestException>();
        exception.Which.StatusCode.Should().Be(ErrorStatusCode.NotFound);
    }
    
    [Fact]
    public async Task ExecuteAsync_Should_Update_Player_And_Currency_When_Success()
    {
        var gameData = GameData.Create(1, "Player");
        var currencyData = Currency.Create(1);

        _dataRepositoryMock
            .Setup(x => x.FindByUserIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(gameData);

        _currencyRepositoryMock
            .Setup(x => x.FindByUserIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(currencyData);

        _levelStoreMock
            .Setup(x => x.GetLevelDefinition(It.IsAny<int>()))
            .Returns(new LevelDefinition(1, 100, 50));

        var useCase = CreateUseCase();
        var command = new GainPlayerExpCommand(1, 50);

        var result = await useCase.ExecuteAsync(command, CancellationToken.None);

        result.Should().NotBeNull();

        _dataRepositoryMock.Verify(x =>
                x.UpdateAsync(It.IsAny<GameData>(), It.IsAny<CancellationToken>()),
            Times.Once);

        _currencyRepositoryMock.Verify(x =>
                x.UpdateAsync(It.IsAny<Currency>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    [Fact]
    public async Task ExecuteWithTransactionAsync_Should_Call_TransactionScope()
    {
        var gameData = GameData.Create(1, "Player");
        var currencyData = Currency.Create(1);

        _dataRepositoryMock
            .Setup(x => x.FindByUserIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(gameData);

        _currencyRepositoryMock
            .Setup(x => x.FindByUserIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(currencyData);

        _levelStoreMock
            .Setup(x => x.GetLevelDefinition(It.IsAny<int>()))
            .Returns(new LevelDefinition(1, 100, 50));

        _transactionScopeMock
            .Setup(x => x.ExecuteAsync(
                It.IsAny<Func<CancellationToken, Task<GainPlayerExpUseCaseResult>>>(),
                It.IsAny<CancellationToken>()))
            .Returns<Func<CancellationToken, Task<GainPlayerExpUseCaseResult>>, CancellationToken>(
                async (action, token) => await action(token));

        var useCase = CreateUseCase();
        var command = new GainPlayerExpCommand(1, 100);

        await useCase.ExecuteWithTransactionAsync(command, CancellationToken.None);

        _transactionScopeMock.Verify(x =>
                x.ExecuteAsync(
                    It.IsAny<Func<CancellationToken, Task<GainPlayerExpUseCaseResult>>>(),
                    It.IsAny<CancellationToken>()),
            Times.Once);
    }
}