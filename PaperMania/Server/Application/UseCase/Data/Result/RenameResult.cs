namespace Server.Application.UseCase.Data.Result;

public record RenameResult(
    int? UserId,
    string NewName);