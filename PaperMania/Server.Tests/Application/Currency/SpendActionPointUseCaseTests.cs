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

public class SpendActionPointUseCaseTests
{
    private readonly Mock<ICurrencyRepository> _repositoryMock = new();
    private readonly Mock<ITransactionScope> _transactionScopeMock = new();

    private SpendActionPointUseCase CreateUseCase()
        => new(_repositoryMock.Object, _transactionScopeMock.Object);

    [Fact]
    public async Task ExecuteAsync_Should_Throw_When_User_Not_Found()
    {
        var command = new SpendActionPointCommand(1, 10);

        _transactionScopeMock
            .Setup(x => x.ExecuteAsync(It.IsAny<Func<CancellationToken, Task<SpendActionPointResult>>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<CancellationToken, Task<SpendActionPointResult>>, CancellationToken>((func, ct) => func(ct));

        _repositoryMock
            .Setup(x => x.FindByUserIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CurrencyData?)null);

        var useCase = CreateUseCase();

        Func<Task> act = () => useCase.ExecuteAsync(command, CancellationToken.None);

        var exception = await act.Should().ThrowAsync<RequestException>();
        exception.Which.StatusCode.Should().Be(ErrorStatusCode.NotFound);
        exception.Which.Message.Should().Be("PLAYER_NOT_FOUND");
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_When_ActionPoint_Insufficient()
    {
        var command = new SpendActionPointCommand(1, 50);

        var currency = CurrencyData.Create(1);
        currency.SetActionPoint(30);

        _transactionScopeMock
            .Setup(x => x.ExecuteAsync(It.IsAny<Func<CancellationToken, Task<SpendActionPointResult>>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<CancellationToken, Task<SpendActionPointResult>>, CancellationToken>((func, ct) => func(ct));

        _repositoryMock
            .Setup(x => x.FindByUserIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(currency);

        var useCase = CreateUseCase();

        Func<Task> act = () => useCase.ExecuteAsync(command, CancellationToken.None);

        var exception = await act.Should().ThrowAsync<RequestException>();
        exception.Which.StatusCode.Should().Be(ErrorStatusCode.BadRequest);
        exception.Which.Message.Should().Be("INSUFFICIENT_ACTION_POINT");
    }

    [Fact]
    public async Task ExecuteAsync_Should_Spend_ActionPoint_When_Sufficient()
    {
        var command = new SpendActionPointCommand(1, 20);

        var currency = CurrencyData.Create(1);
        currency.SetActionPoint(50);

        _repositoryMock
            .Setup(x => x.FindByUserIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(currency);

        _repositoryMock
            .Setup(x => x.UpdateAsync(currency, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _transactionScopeMock
            .Setup(x => x.ExecuteAsync(It.IsAny<Func<CancellationToken, Task<SpendActionPointResult>>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<CancellationToken, Task<SpendActionPointResult>>, CancellationToken>((func, ct) => func(ct));

        var useCase = CreateUseCase();

        var result = await useCase.ExecuteAsync(command, CancellationToken.None);

        _repositoryMock.Verify(x => x.UpdateAsync(currency, It.IsAny<CancellationToken>()), Times.Once);
        result.ActionPoint.Should().Be(30);
    }
}