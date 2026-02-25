using FluentAssertions;
using Moq;
using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.Output.Persistence;
using Server.Application.UseCase.Currency;
using Server.Application.UseCase.Currency.Command;
using Server.Domain.Entity;

namespace Server.Tests.Application.Currency;

public class GetCurrencyDataUseCaseTests
{
    private readonly Mock<ICurrencyRepository> _repositoryMock = new();

    private GetCurrencyDataUseCase CreateUseCase()
        => new(_repositoryMock.Object);

    [Fact]
    public async Task ExecuteAsync_Should_Throw_When_Data_Not_Found()
    {
        var command = new GetCurrencyDataCommand(1);

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
    public async Task ExecuteAsync_Should_Return_CurrencyData_When_Found()
    {
        var command = new GetCurrencyDataCommand(1);

        var currency = CurrencyData.Create(1);
        currency.SetActionPoint(100);
        currency.SetGold(200);
        currency.SetPaperPiece(300);

        _repositoryMock
            .Setup(x => x.FindByUserIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(currency);

        var useCase = CreateUseCase();

        var result = await useCase.ExecuteAsync(command, CancellationToken.None);

        result.ActionPoint.Should().Be(100);
        result.Gold.Should().Be(200);
        result.PaperPiece.Should().Be(300);
    }
}