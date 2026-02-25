using FluentAssertions;
using Moq;
using Server.Application.Port.Output.Persistence;
using Server.Application.UseCase.Reward;
using Server.Application.UseCase.Reward.Command;
using Server.Infrastructure.Persistence.Model;

namespace Server.Tests.Application.Reward;

public class CheckStageClearedUseCaseTests
{
    private readonly Mock<IStageRepository> _stageRepoMock = new();

    private CheckStageClearedUseCase CreateUseCase() => new(_stageRepoMock.Object);

    [Fact]
    public async Task ExecuteAsync_Should_Return_True_When_StageData_Exists()
    {
        var command = new CheckStageClearedCommand(1, 1, 1);

        _stageRepoMock
            .Setup(x => x.FindByUserIdAsync(1, 1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PlayerStageData(1, 1, 1));

        var useCase = CreateUseCase();

        var result = await useCase.ExecuteAsync(command, CancellationToken.None);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_False_When_StageData_Not_Exists()
    {
        var command = new CheckStageClearedCommand(1, 1, 1);

        _stageRepoMock
            .Setup(x => x.FindByUserIdAsync(1, 1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PlayerStageData?)null);

        var useCase = CreateUseCase();

        var result = await useCase.ExecuteAsync(command, CancellationToken.None);

        result.Should().BeFalse();
    }
}