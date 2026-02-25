using FluentAssertions;
using Moq;
using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.Output.StaticData;
using Server.Application.UseCase.Reward;
using Server.Application.UseCase.Reward.Command;
using Server.Infrastructure.StaticData.Model;

namespace Server.Tests.Application.Reward;

public class GetStageRewardUseCaseTests
{
    private readonly Mock<IStageRewardStore> _storeMock = new();

    private GetStageRewardUseCase CreateUseCase() => new(_storeMock.Object);

    [Fact]
    public void Execute_Should_Return_StageReward_When_Found()
    {
        var command = new GetStageRewardCommand(1, 1);
        var reward = new StageReward { Gold = 100, PaperPiece = 50, ClearExp = 200 };

        _storeMock.Setup(x => x.GetStageReward(1, 1)).Returns(reward);

        var useCase = CreateUseCase();

        var result = useCase.Execute(command);

        result.Should().Be(reward);
    }

    [Fact]
    public void Execute_Should_Throw_When_StageReward_Not_Found()
    {
        var command = new GetStageRewardCommand(1, 1);

        _storeMock.Setup(x => x.GetStageReward(1, 1)).Returns((StageReward?)null);

        var useCase = CreateUseCase();

        Action act = () => useCase.Execute(command);

        var exception = act.Should().Throw<RequestException>().Which;
        exception.StatusCode.Should().Be(ErrorStatusCode.NotFound);
        exception.Message.Should().Be("STAGE_NOT_FOUND");
    }
}