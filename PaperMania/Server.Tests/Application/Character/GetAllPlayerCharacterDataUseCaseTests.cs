using FluentAssertions;
using Moq;
using Server.Application.Exceptions;
using Server.Application.Port.Output.Persistence;
using Server.Application.UseCase.Character;
using Server.Infrastructure.Persistence.Model;

namespace Server.Tests.Application.Character;

public class GetAllPlayerCharacterDataUseCaseTests
{
    private readonly Mock<ICharacterRepository> _repoMock = new();

    private GetAllPlayerCharacterDataUseCase CreateUseCase()
        => new GetAllPlayerCharacterDataUseCase(_repoMock.Object);

    [Fact]
    public async Task ExecuteAsync_Should_Throw_When_UserId_Invalid()
    {
        var useCase = CreateUseCase();

        Func<Task> act = () => useCase.ExecuteAsync(0, CancellationToken.None);

        var exception = await act.Should().ThrowAsync<RequestException>();
        exception.Which.StatusCode.Should().Be(Server.Api.Dto.Response.ErrorStatusCode.BadRequest);
        exception.Which.Message.Should().Be("INVALID_USER_ID");
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_EmptyList_When_No_Characters()
    {
        int userId = 1;
        _repoMock.Setup(x => x.FindAll(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Enumerable.Empty<PlayerCharacterData>());

        var useCase = CreateUseCase();

        var result = await useCase.ExecuteAsync(userId, CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_All_Characters()
    {
        int userId = 1;
        var characters = new List<PlayerCharacterData>
        {
            new PlayerCharacterData(userId, 10),
            new PlayerCharacterData(userId, 20)
        };

        _repoMock.Setup(x => x.FindAll(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(characters);

        var useCase = CreateUseCase();

        var result = await useCase.ExecuteAsync(userId, CancellationToken.None);

        result.Should().HaveCount(2);
        result.Should().ContainEquivalentOf(characters[0]);
        result.Should().ContainEquivalentOf(characters[1]);
    }
}