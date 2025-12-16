namespace Server.Application.UseCase.Data.Command;

public record RenameCommand(
    int? UserId,
    string NewName);