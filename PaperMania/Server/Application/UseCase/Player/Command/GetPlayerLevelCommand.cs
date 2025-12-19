namespace Server.Application.UseCase.Player.Command;

public record GetPlayerLevelCommand(
    int? UserId
    );