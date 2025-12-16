namespace Server.Application.UseCase.Data.Command;

public record GetPlayerNameByUserIdCommand(
    int? UserId
    );