namespace Server.Application.UseCase.Player.Command;

public record GetPlayerNameCommand(
    int? UserId
    );