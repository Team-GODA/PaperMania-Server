namespace Server.Application.UseCase.Data.Command;

public record GetPlayerLevelByUserIdCommand(
    int? UserId
    );