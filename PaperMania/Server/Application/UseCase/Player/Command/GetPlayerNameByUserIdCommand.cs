namespace Server.Application.UseCase.Player.Command;

public record GetPlayerNameByUserIdCommand(
    int? UserId
    );