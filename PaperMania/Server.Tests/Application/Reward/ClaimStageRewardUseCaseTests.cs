using FluentAssertions;
using Moq;
using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.Input.Currency;
using Server.Application.Port.Input.Player;
using Server.Application.Port.Input.Reward;
using Server.Application.Port.Output.Persistence;
using Server.Application.Port.Output.StaticData;
using Server.Application.Port.Output.Transaction;
using Server.Application.UseCase.Currency.Command;
using Server.Application.UseCase.Player.Command;
using Server.Application.UseCase.Reward;
using Server.Application.UseCase.Reward.Command;
using Server.Application.UseCase.Reward.Result;
using Server.Domain.Entity;
using Server.Infrastructure.Persistence.Model;
using Server.Infrastructure.StaticData.Model;

namespace Server.Tests.Application.Reward;

public class ClaimStageRewardUseCaseTests
{
    private readonly Mock<IStageRepository> _stageRepoMock = new();
    private readonly Mock<ICurrencyRepository> _currencyRepoMock = new();
    private readonly Mock<IDataRepository> _dataRepoMock = new();
    private readonly Mock<IStageRewardStore> _stageRewardStoreMock = new();
    private readonly Mock<ICheckStageClearedUseCase> _checkClearedUseCaseMock = new();
    private readonly Mock<IGainGoldUseCase> _gainGoldUseCaseMock = new();
    private readonly Mock<IGainPaperPieceUseCase> _gainPaperPieceUseCaseMock = new();
    private readonly Mock<IGainPlayerExpUseCase> _gainPlayerExpUseCaseMock = new();
    private readonly Mock<ITransactionScope> _transactionScopeMock = new();

    private ClaimStageRewardUseCase CreateUseCase()
        => new(
            _stageRepoMock.Object,
            _currencyRepoMock.Object,
            _dataRepoMock.Object,
            _stageRewardStoreMock.Object,
            _checkClearedUseCaseMock.Object,
            _gainGoldUseCaseMock.Object,
            _gainPaperPieceUseCaseMock.Object,
            _gainPlayerExpUseCaseMock.Object,
            _transactionScopeMock.Object
        );

    [Fact]
    public async Task ExecuteAsync_Should_Throw_When_StageReward_Not_Found()
    {
        var command = new ClaimStageRewardCommand(1, 1, 1);

        _stageRewardStoreMock
            .Setup(x => x.GetStageReward(1, 1))
            .Returns((StageReward?)null);

        var useCase = CreateUseCase();

        Func<Task> act = () => useCase.ExecuteAsync(command, CancellationToken.None);

        var exception = await act.Should().ThrowAsync<RequestException>();
        exception.Which.StatusCode.Should().Be(ErrorStatusCode.NotFound);
        exception.Which.Message.Should().Be("STAGE_REWARD_NOT_FOUND");
    }

    [Fact]
    public async Task ExecuteAsync_Should_Claim_Reward_On_First_Clear()
    {
        var command = new ClaimStageRewardCommand(1, 1, 1);

        var stageReward = new StageReward { Gold = 100, PaperPiece = 50, ClearExp = 200 };
        _stageRewardStoreMock.Setup(x => x.GetStageReward(1, 1)).Returns(stageReward);

        _checkClearedUseCaseMock
            .Setup(x => x.ExecuteAsync(It.IsAny<CheckStageClearedCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var currencyData = CurrencyData.Create(1);
        currencyData.SetGold(0);
        currencyData.SetPaperPiece(0);

        var playerData = PlayerData.Create(1, "TestPlayer");

        _currencyRepoMock.Setup(x => x.FindByUserIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(currencyData);
        _dataRepoMock.Setup(x => x.FindByUserIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(playerData);

        _transactionScopeMock
            .Setup(x => x.ExecuteAsync(It.IsAny<Func<CancellationToken, Task<ClaimStageRewardResult>>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<CancellationToken, Task<ClaimStageRewardResult>>, CancellationToken>((func, ct) => func(ct));

        var useCase = CreateUseCase();

        var result = await useCase.ExecuteAsync(command, CancellationToken.None);

        _gainGoldUseCaseMock.Verify(x => x.ExecuteAsync(It.IsAny<GainGoldCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        _gainPaperPieceUseCaseMock.Verify(x => x.ExecuteAsync(It.IsAny<GainPaperPieceCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        _gainPlayerExpUseCaseMock.Verify(x => x.ExecuteAsync(It.IsAny<GainPlayerExpCommand>(), It.IsAny<CancellationToken>()), Times.Once);

        _stageRepoMock.Verify(x => x.CreateAsync(It.IsAny<PlayerStageData>(), It.IsAny<CancellationToken>()), Times.Once);

        result.IsCleared.Should().BeFalse();
        result.Gold.Should().Be(currencyData.Gold);
        result.PaperPiece.Should().Be(currencyData.PaperPiece);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Claim_Reward_When_Already_Cleared()
    {
        var command = new ClaimStageRewardCommand(1, 1, 1);

        var stageReward = new StageReward { Gold = 100, PaperPiece = 50, ClearExp = 200 };
        _stageRewardStoreMock.Setup(x => x.GetStageReward(1, 1)).Returns(stageReward);

        _checkClearedUseCaseMock
            .Setup(x => x.ExecuteAsync(It.IsAny<CheckStageClearedCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var currencyData = CurrencyData.Create(1);
        currencyData.SetGold(0);
        currencyData.SetPaperPiece(0);

        var playerData = PlayerData.Create(1, "TestPlayer");

        _currencyRepoMock.Setup(x => x.FindByUserIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(currencyData);
        _dataRepoMock.Setup(x => x.FindByUserIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(playerData);

        _transactionScopeMock
            .Setup(x => x.ExecuteAsync(It.IsAny<Func<CancellationToken, Task<ClaimStageRewardResult>>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<CancellationToken, Task<ClaimStageRewardResult>>, CancellationToken>((func, ct) => func(ct));

        var useCase = CreateUseCase();

        var result = await useCase.ExecuteAsync(command, CancellationToken.None);

        _gainGoldUseCaseMock.Verify(x => x.ExecuteAsync(It.IsAny<GainGoldCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        _gainPaperPieceUseCaseMock.Verify(x => x.ExecuteAsync(It.IsAny<GainPaperPieceCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        _gainPlayerExpUseCaseMock.Verify(x => x.ExecuteAsync(It.IsAny<GainPlayerExpCommand>(), It.IsAny<CancellationToken>()), Times.Once);

        _stageRepoMock.Verify(x => x.CreateAsync(It.IsAny<PlayerStageData>(), It.IsAny<CancellationToken>()), Times.Never);

        result.IsCleared.Should().BeTrue();
    }
}