namespace Server.Application.UseCase.Player.Command;

public record RenameCommand(
    int? UserId,
    string NewName);