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

public class UpdateMaxActionPointUseCaseTests
{
    private readonly Mock<ICurrencyRepository> _repositoryMock = new();
    private readonly Mock<ITransactionScope> _transactionScopeMock = new();

    private UpdateMaxActionPointUseCase CreateUseCase()
        => new(_repositoryMock.Object, _transactionScopeMock.Object);

    [Fact]
    public async Task ExecuteAsync_Should_Throw_When_User_Not_Found()
    {
        var command = new UpdateMaxActionPointCommand(1, 200);

        _transactionScopeMock
            .Setup(x => x.ExecuteAsync(It.IsAny<Func<CancellationToken, Task<UpdateMaxActionPointResult>>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<CancellationToken, Task<UpdateMaxActionPointResult>>, CancellationToken>((func, ct) => func(ct));

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
    public async Task ExecuteAsync_Should_Update_MaxActionPoint()
    {
        var command = new UpdateMaxActionPointCommand(1, 200);

        var currency = CurrencyData.Create(1);
        currency.MaxActionPoint = 100;

        _repositoryMock
            .Setup(x => x.FindByUserIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(currency);

        _repositoryMock
            .Setup(x => x.UpdateAsync(currency, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _transactionScopeMock
            .Setup(x => x.ExecuteAsync(It.IsAny<Func<CancellationToken, Task<UpdateMaxActionPointResult>>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<CancellationToken, Task<UpdateMaxActionPointResult>>, CancellationToken>((func, ct) => func(ct));

        var useCase = CreateUseCase();

        var result = await useCase.ExecuteAsync(command, CancellationToken.None);

        _repositoryMock.Verify(x => x.UpdateAsync(currency, It.IsAny<CancellationToken>()), Times.Once);
        result.MaxActionPoint.Should().Be(200);
    }
}