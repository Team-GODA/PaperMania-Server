using FluentAssertions;
using Moq;
using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.Output.Service;
using Server.Application.UseCase.Auth;

namespace Server.Tests.Application.Auth;

public class ValidateUseCaseTests
{
    private readonly Mock<ISessionService> _sessionServiceMock = new();

    private ValidateUseCase CreateUseCase()
    {
        return new ValidateUseCase(
            _sessionServiceMock.Object
        );
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_When_SessionId_Is_Null_Or_Empty()
    {
        var useCase = CreateUseCase();

        Func<Task> act = async () =>
            await useCase.ExecuteAsync("", CancellationToken.None);

        var exception = await act.Should()
            .ThrowAsync<RequestException>();

        exception.Which.StatusCode
            .Should().Be(ErrorStatusCode.Unauthorized);

        _sessionServiceMock.Verify(x =>
            x.ValidateSessionAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_When_Session_Is_Invalid()
    {
        _sessionServiceMock
            .Setup(x => x.ValidateSessionAsync("SESSION123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var useCase = CreateUseCase();

        Func<Task> act = async () =>
            await useCase.ExecuteAsync("SESSION123", CancellationToken.None);

        var exception = await act.Should()
            .ThrowAsync<RequestException>();

        exception.Which.StatusCode
            .Should().Be(ErrorStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Not_Throw_When_Session_Is_Valid()
    {
        _sessionServiceMock
            .Setup(x => x.ValidateSessionAsync("SESSION123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var useCase = CreateUseCase();

        Func<Task> act = async () =>
            await useCase.ExecuteAsync("SESSION123", CancellationToken.None);

        await act.Should().NotThrowAsync();

        _sessionServiceMock.Verify(x =>
            x.ValidateSessionAsync("SESSION123", It.IsAny<CancellationToken>()),
            Times.Once);
    }
}