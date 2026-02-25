using FluentAssertions;
using Moq;
using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.Output.Cache;
using Server.Application.Port.Output.Persistence;
using Server.Application.UseCase.Player;
using Server.Application.UseCase.Player.Command;
using Server.Domain.Entity;

namespace Server.Tests.Application.Player;

public class GetPlayerNameUseCaseTests
{
    private readonly Mock<IDataRepository> _repositoryMock = new();
    private readonly Mock<ICacheAsideService> _cacheMock = new();

    private GetPlayerNameUseCase CreateUseCase() =>
        new(_repositoryMock.Object, _cacheMock.Object);

    [Fact]
    public async Task ExecuteAsync_Should_Throw_When_Player_Not_Found()
    {
        _cacheMock
            .Setup(x => x.GetOrSetAsync<PlayerData>(
                It.IsAny<string>(),
                It.IsAny<Func<CancellationToken, Task<PlayerData?>>>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((PlayerData?)null);

        var useCase = CreateUseCase();
        var command = new GetPlayerNameCommand(1);

        Func<Task> act = () => useCase.ExecuteAsync(command, CancellationToken.None);

        var exception = await act.Should().ThrowAsync<RequestException>();
        exception.Which.StatusCode.Should().Be(ErrorStatusCode.NotFound);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Player_Name_When_Success()
    {
        var player = PlayerData.Create(1, "TestPlayer");

        _cacheMock
            .Setup(x => x.GetOrSetAsync<PlayerData>(
                It.IsAny<string>(),
                It.IsAny<Func<CancellationToken, Task<PlayerData?>>>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(player);

        var useCase = CreateUseCase();
        var command = new GetPlayerNameCommand(1);

        var result = await useCase.ExecuteAsync(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.PlayerName.Should().Be("TestPlayer");
    }
}