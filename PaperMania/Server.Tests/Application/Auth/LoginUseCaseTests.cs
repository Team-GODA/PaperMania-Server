using FluentAssertions;
using Moq;
using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.Output.Cache;
using Server.Application.Port.Output.Persistence;
using Server.Application.Port.Output.Service;
using Server.Application.UseCase.Auth;
using Server.Application.UseCase.Auth.Command;
using Server.Domain.Entity;
using Server.Domain.Service;

namespace Server.Tests.Application.Auth;

public class LoginUseCaseTests
{
    private readonly Mock<IAccountRepository> _repositoryMock = new();
    private readonly Mock<ISessionService> _sessionServiceMock = new();
    private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
    private readonly Mock<ICacheAsideService> _cacheAsideMock = new();
    
    private LoginUseCase CreateUseCase()
    {
        return new LoginUseCase(
            _repositoryMock.Object,
            _sessionServiceMock.Object,
            _passwordHasherMock.Object,
            _cacheAsideMock.Object
        );
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_When_Account_Not_Found()
    {
        _cacheAsideMock
            .Setup(x => x.GetOrSetAsync(
                It.IsAny<string>(),
                It.IsAny<Func<CancellationToken, Task<Account?>>>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Account?)null);
        
        var useCase = CreateUseCase();
        var command = new LoginCommand("player1", "password");

        Func<Task> act = async () =>
            await useCase.ExecuteAsync(command, CancellationToken.None);

        var exception = await act.Should()
            .ThrowAsync<RequestException>();

        exception.Which.StatusCode
            .Should().Be(ErrorStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_When_Password_Invalid()
    {
        var account = new Account("player1", "email", "HASHED", false) { Id = 1 };
        
        _cacheAsideMock
            .Setup(x => x.GetOrSetAsync(
                It.IsAny<string>(),
                It.IsAny<Func<CancellationToken, Task<Account?>>>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);
        
        _passwordHasherMock
            .Setup(x => x.Verify("wrong", "HASHED"))
            .Returns(false);
        
        var useCase = CreateUseCase();
        var command = new LoginCommand("player1", "wrong");

        Func<Task> act = async () =>
            await useCase.ExecuteAsync(command, CancellationToken.None);

        var exception = await act.Should()
            .ThrowAsync<RequestException>();

        exception.Which.StatusCode
            .Should().Be(ErrorStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task ExecuteAsync_Should_Throw_When_Session_Create_Fails()
    {
        var account = new Account("player1", "email", "HASHED", true) { Id = 1 };

        _cacheAsideMock
            .Setup(x => x.GetOrSetAsync(
                It.IsAny<string>(),
                It.IsAny<Func<CancellationToken, Task<Account?>>>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);

        _passwordHasherMock
            .Setup(x => x.Verify("password", "HASHED"))
            .Returns(true);

        _sessionServiceMock
            .Setup(x => x.CreateSessionAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(string.Empty);
        
        var useCase = CreateUseCase();
        var command = new LoginCommand("player1", "password");

        Func<Task> act = async () =>
            await useCase.ExecuteAsync(command, CancellationToken.None);

        var exception = await act.Should()
            .ThrowAsync<RequestException>();

        exception.Which.StatusCode
            .Should().Be(ErrorStatusCode.ServerError);
    }
    
    [Fact]
    public async Task ExecuteAsync_Should_Return_LoginResult_When_Success()
    {
        var account = new Account("player1", "email", "HASHED", true) { Id = 1 };

        _cacheAsideMock
            .Setup(x => x.GetOrSetAsync(
                It.IsAny<string>(),
                It.IsAny<Func<CancellationToken, Task<Account?>>>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);

        _passwordHasherMock
            .Setup(x => x.Verify("password", "HASHED"))
            .Returns(true);

        _sessionServiceMock
            .Setup(x => x.CreateSessionAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync("SESSION123");

        var useCase = CreateUseCase();
        var command = new LoginCommand("player1", "password");

        var result = await useCase.ExecuteAsync(command, CancellationToken.None);

        result.SessionId.Should().Be("SESSION123");
    }
}