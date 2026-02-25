using FluentAssertions;
using Moq;
using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.Output.Persistence;
using Server.Application.Port.Output.Service;
using Server.Application.UseCase.Auth;
using Server.Application.UseCase.Auth.Command;
using Server.Domain.Entity;
using Server.Domain.Service;

namespace Server.Tests.Application.Auth;

public class RegisterUseCaseTests
{
    private readonly Mock<IAccountRepository> _repositoryMock = new();
    private readonly Mock<IPasswordHasher> _passwordHasherMock = new();

    private RegisterUseCase CreateUseCase()
    {
        return new RegisterUseCase(
            _repositoryMock.Object,
            _passwordHasherMock.Object
        );
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_When_PlayerId_Already_Exists()
    {
        _repositoryMock
            .Setup(x => x.ExistsByPlayerIdAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var useCase = CreateUseCase();
        var command = new RegisterCommand("player1", "test@test.com", "1234");

        Func<Task> act = async () =>
            await useCase.ExecuteAsync(command, CancellationToken.None);

        var exception = await act.Should()
            .ThrowAsync<RequestException>();

        exception.Which.StatusCode
            .Should().Be(ErrorStatusCode.Conflict);

        _repositoryMock.Verify(x =>
            x.CreateAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Create_Account_When_Success()
    {
        _repositoryMock
            .Setup(x => x.ExistsByPlayerIdAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _passwordHasherMock
            .Setup(x => x.Hash("1234"))
            .Returns("HASHED_PASSWORD");

        var useCase = CreateUseCase();
        var command = new RegisterCommand("player1", "test@test.com", "1234");

        await useCase.ExecuteAsync(command, CancellationToken.None);

        _repositoryMock.Verify(x =>
            x.CreateAsync(
                It.Is<Account>(a =>
                    a.PlayerId == "player1" &&
                    a.Email == "test@test.com" &&
                    a.Password == "HASHED_PASSWORD" &&
                    a.IsNewAccount == true &&
                    a.Role == "user"
                ),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _passwordHasherMock.Verify(x =>
            x.Hash("1234"),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Call_Validate()
    {
        var useCase = CreateUseCase();

        var invalidCommand = new RegisterCommand("", "", "");

        Func<Task> act = async () =>
            await useCase.ExecuteAsync(invalidCommand, CancellationToken.None);

        await act.Should().ThrowAsync<Exception>();

        _repositoryMock.Verify(x =>
            x.ExistsByPlayerIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}