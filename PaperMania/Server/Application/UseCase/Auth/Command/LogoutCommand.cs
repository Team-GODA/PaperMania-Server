namespace Server.Application.UseCase.Auth.Command;

public record LogoutCommand(
    string SessionId
    );