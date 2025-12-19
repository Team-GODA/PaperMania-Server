namespace Server.Application.UseCase.Player.Result;

public record RenameResult(
    int? UserId,
    string NewName);