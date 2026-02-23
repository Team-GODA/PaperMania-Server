using Moq;
using Server.Application.Port.Output.Service;
using Server.Application.UseCase.Auth;

namespace Server.Tests.Application.Auth;

public class LogoutUseCaseTests
{
    private readonly Mock<ISessionService> _sessionServiceMock = new();

    private LogoutUseCase CreateUseCase()
    {
        return new LogoutUseCase(
            _sessionServiceMock.Object
        );
    }
    
    [Fact]
    public async Task ExecuteAsync_Should_Delete_Session_When_SessionId_Is_Valid()
    {
        var useCase = CreateUseCase();

        await useCase.ExecuteAsync("SESSION123", CancellationToken.None);

        _sessionServiceMock.Verify(x =>
                x.DeleteSessionAsync("SESSION123", It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    [Fact]
    public async Task ExecuteAsync_Should_Not_Delete_Session_When_SessionId_Is_Null_Or_Empty()
    {
        var useCase = CreateUseCase();

        await useCase.ExecuteAsync("", CancellationToken.None);

        _sessionServiceMock.Verify(x =>
                x.DeleteSessionAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}