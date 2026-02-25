using FluentAssertions;
using Moq;
using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.Output.Persistence;
using Server.Application.Port.Output.Service;
using Server.Application.UseCase.Currency;
using Server.Application.UseCase.Currency.Command;
using Server.Domain.Entity;

namespace Server.Tests.Application.Currency;

public class GetActionPointUseCaseTests
{
    private readonly Mock<ICurrencyRepository> _repositoryMock = new();
    private readonly Mock<IActionPointService> _apServiceMock = new();

    private GetActionPointUseCase CreateUseCase()
        => new(_repositoryMock.Object, _apServiceMock.Object);

    [Fact]
    public async Task ExecuteAsync_Should_Throw_When_Data_Not_Found()
    {
        var command = new GetActionPointCommand(1);

        _repositoryMock
            .Setup(x => x.FindByUserIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CurrencyData?)null);

        var useCase = CreateUseCase();

        Func<Task> act = () => useCase.ExecuteAsync(command, CancellationToken.None);

        var exception = await act.Should().ThrowAsync<RequestException>();
        exception.Which.StatusCode.Should().Be(ErrorStatusCode.NotFound);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Current_AP_When_Not_Regenerated()
    {
        var command = new GetActionPointCommand(1);

        var currency = CurrencyData.Create(1);
        currency.SetActionPoint(50);

        _repositoryMock
            .Setup(x => x.FindByUserIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(currency);

        _apServiceMock
            .Setup(x => x.TryRegenerate(currency, It.IsAny<DateTime>()))
            .Returns(false);

        var useCase = CreateUseCase();

        var result = await useCase.ExecuteAsync(command, CancellationToken.None);

        _repositoryMock.Verify(x =>
            x.UpdateAsync(It.IsAny<CurrencyData>(), It.IsAny<CancellationToken>()),
            Times.Never);

        result.ActionPoint.Should().Be(50);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Update_When_Regenerated()
    {
        var command = new GetActionPointCommand(1);

        var currency = CurrencyData.Create(1);
        currency.SetActionPoint(40);

        _repositoryMock
            .Setup(x => x.FindByUserIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(currency);

        _apServiceMock
            .Setup(x => x.TryRegenerate(currency, It.IsAny<DateTime>()))
            .Returns(true);

        var useCase = CreateUseCase();

        var result = await useCase.ExecuteAsync(command, CancellationToken.None);

        _repositoryMock.Verify(x =>
            x.UpdateAsync(currency, It.IsAny<CancellationToken>()),
            Times.Once);

        result.ActionPoint.Should().Be(currency.ActionPoint);
    }
}