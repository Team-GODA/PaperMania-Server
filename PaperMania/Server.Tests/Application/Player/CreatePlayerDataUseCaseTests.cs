using FluentAssertions;
using Moq;
using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.Output.Persistence;
using Server.Application.Port.Output.Service;
using Server.Application.Port.Output.Transaction;
using Server.Application.UseCase.Player;
using Server.Application.UseCase.Player.Command;
using Server.Domain.Entity;

namespace Server.Tests.Application.Player;

public class CreatePlayerDataUseCaseTests
{
    private readonly Mock<IDataRepository> _dataRepositoryMock = new();
    private readonly Mock<IAccountRepository> _accountRepositoryMock = new();
    private readonly Mock<ICurrencyRepository> _currencyRepositoryMock = new();
    private readonly Mock<ISessionService> _sessionServiceMock = new();
    private readonly Mock<ITransactionScope> _transactionScopeMock = new();
    
    private CreatePlayerDataUseCase CreateUseCase() => new(
        _dataRepositoryMock.Object,
        _accountRepositoryMock.Object,
        _currencyRepositoryMock.Object,
        _sessionServiceMock.Object,
        _transactionScopeMock.Object
    );
    
    [Fact]
    public async Task ExecuteAsync_Should_Throw_When_Account_Not_Found()
    {
        _sessionServiceMock
            .Setup(x => x.FindUserIdBySessionIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _accountRepositoryMock
            .Setup(x => x.FindByUserIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Account?)null);

        var useCase = CreateUseCase();
        var command = new AddPlayerDataCommand("SESSION", "Player1");

        Func<Task> act = () => useCase.ExecuteAsync(command, CancellationToken.None);

        var exception = await act.Should().ThrowAsync<RequestException>();
        exception.Which.StatusCode.Should().Be(ErrorStatusCode.NotFound);
    }
    
    [Fact]
    public async Task ExecuteAsync_Should_Throw_When_PlayerData_Already_Exists()
    {
        var account = new Account( 
            "player1",
            "email",
            "HASHED", 
            false  
        );
        
        _sessionServiceMock
            .Setup(x => x.FindUserIdBySessionIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _accountRepositoryMock
            .Setup(x => x.FindByUserIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);

        var useCase = CreateUseCase();
        var command = new AddPlayerDataCommand("SESSION", "Player1");

        Func<Task> act = () => useCase.ExecuteAsync(command, CancellationToken.None);

        var exception = await act.Should().ThrowAsync<RequestException>();
        exception.Which.StatusCode.Should().Be(ErrorStatusCode.Conflict);
    }
    
    [Fact]
    public async Task ExecuteAsync_Should_Create_PlayerData_When_Success()
    {
        var account = new Account( 
            "player1",
            "email",
            "HASHED", 
            true
            );

        _sessionServiceMock
            .Setup(x => x.FindUserIdBySessionIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _accountRepositoryMock
            .Setup(x => x.FindByUserIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);

        _transactionScopeMock
            .Setup(x => x.ExecuteAsync(
                It.IsAny<Func<CancellationToken, Task>>(),
                It.IsAny<CancellationToken>()))
            .Returns<Func<CancellationToken, Task>, CancellationToken>(
                async (action, token) => await action(token));

        var useCase = CreateUseCase();
        var command = new AddPlayerDataCommand("Player1", "SESSION");

        var result = await useCase.ExecuteAsync(command, CancellationToken.None);

        result.PlayerName.Should().Be("Player1");

        _dataRepositoryMock.Verify(x =>
                x.CreateAsync(It.IsAny<GameData>(), It.IsAny<CancellationToken>()),
            Times.Once);

        _currencyRepositoryMock.Verify(x =>
                x.CreateByUserIdAsync(1, It.IsAny<CancellationToken>()),
            Times.Once);

        _accountRepositoryMock.Verify(x =>
                x.UpdateAsync(
                    It.Is<Account>(a => a.IsNewAccount == false),
                    It.IsAny<CancellationToken>()),
            Times.Once);
    }
}