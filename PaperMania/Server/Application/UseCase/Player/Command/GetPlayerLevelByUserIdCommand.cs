namespace Server.Application.UseCase.Player.Command;

public record GetPlayerLevelByUserIdCommand(
    int? UserId
    );