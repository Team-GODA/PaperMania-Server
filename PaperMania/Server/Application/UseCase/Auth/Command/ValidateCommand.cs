namespace Server.Application.UseCase.Auth.Command;

public record ValidateCommand(
    string SessionId
    );