using FluentAssertions;
using Moq;
using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.Output.Persistence;
using Server.Application.UseCase.Player;
using Server.Application.UseCase.Player.Command;
using Server.Domain.Entity;

namespace Server.Tests.Application.Player;

public class RenameUseCaseTests
{
    private readonly Mock<IDataRepository> _repositoryMock = new();

    private RenameUseCase CreateUseCase() =>
        new(_repositoryMock.Object);

    [Fact]
    public async Task ExecuteAsync_Should_Throw_When_Name_Already_Exists()
    {
        var command = new RenameCommand(1, "DuplicatedName");

        _repositoryMock
            .Setup(x => x.ExistsPlayerNameAsync("DuplicatedName", It.IsAny<CancellationToken>()))
            .ReturnsAsync(GameData.Create(999, "DuplicatedName"));

        var useCase = CreateUseCase();

        Func<Task> act = () => useCase.ExecuteAsync(command, CancellationToken.None);

        var exception = await act.Should().ThrowAsync<RequestException>();
        exception.Which.StatusCode.Should().Be(ErrorStatusCode.Conflict);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Rename_When_Name_Is_Available()
    {
        var command = new RenameCommand(1, "NewName");

        _repositoryMock
            .Setup(x => x.ExistsPlayerNameAsync("NewName", It.IsAny<CancellationToken>()))
            .ReturnsAsync((GameData?)null);

        var useCase = CreateUseCase();

        var result = await useCase.ExecuteAsync(command, CancellationToken.None);

        _repositoryMock.Verify(x =>
                x.RenamePlayerNameAsync(1, "NewName", It.IsAny<CancellationToken>()),
            Times.Once);

        result.UserId.Should().Be(1);
        result.NewName.Should().Be("NewName");
    }
}