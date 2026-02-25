using FluentAssertions;
using Moq;
using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.Output.Persistence;
using Server.Application.Port.Output.Transaction;
using Server.Application.UseCase.Currency;
using Server.Application.UseCase.Currency.Command;
using Server.Application.UseCase.Currency.Result;
using Server.Domain.Entity;

namespace Server.Tests.Application.Currency;

public class GainGoldUseCaseTests
{
    private readonly Mock<ICurrencyRepository> _repositoryMock = new();
    private readonly Mock<ITransactionScope> _transactionMock = new();

    private GainGoldUseCase CreateUseCase()
        => new(_repositoryMock.Object, _transactionMock.Object);

    [Fact]
    public async Task ExecuteAsync_Should_Throw_When_Player_Not_Found()
    {
        var command = new GainGoldCommand(1, 100);

        _repositoryMock
            .Setup(x => x.FindByUserIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CurrencyData?)null);

        var useCase = CreateUseCase();

        Func<Task> act = () => useCase.ExecuteAsync(command, CancellationToken.None);

        var exception = await act.Should().ThrowAsync<RequestException>();
        exception.Which.StatusCode.Should().Be(ErrorStatusCode.NotFound);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Add_Gold_When_Player_Exists()
    {
        var initialGold = 500;
        var command = new GainGoldCommand(1, 100);

        var currency = CurrencyData.Create(1, initialGold);

        _repositoryMock
            .Setup(x => x.FindByUserIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(currency);

        var useCase = CreateUseCase();

        var result = await useCase.ExecuteAsync(command, CancellationToken.None);

        currency.Gold.Should().Be(initialGold + 100);

        _repositoryMock.Verify(x =>
            x.UpdateAsync(currency, It.IsAny<CancellationToken>()),
            Times.Once);

        result.Gold.Should().Be(initialGold + 100);
    }

    [Fact]
    public async Task ExecuteWithTransactionAsync_Should_Use_TransactionScope()
    {
        var initialGold = 500;
        var command = new GainGoldCommand(1, 100);

        var currency = CurrencyData.Create(1, initialGold);

        _repositoryMock
            .Setup(x => x.FindByUserIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(currency);

        _transactionMock
            .Setup(x => x.ExecuteAsync(
                It.IsAny<Func<CancellationToken, Task<GainGoldResult>>>(),
                It.IsAny<CancellationToken>()))
            .Returns<Func<CancellationToken, Task<GainGoldResult>>, CancellationToken>(
                async (func, ct) => await func(ct));

        var useCase = CreateUseCase();

        var result = await useCase.ExecuteWithTransactionAsync(command, CancellationToken.None);

        _transactionMock.Verify(x =>
            x.ExecuteAsync(
                It.IsAny<Func<CancellationToken, Task<GainGoldResult>>>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        result.Gold.Should().Be(initialGold + 100);
    }
}