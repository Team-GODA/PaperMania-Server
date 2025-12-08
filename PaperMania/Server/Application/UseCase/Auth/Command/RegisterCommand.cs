namespace Server.Application.UseCase.Auth.Command;

public record RegisterCommand(
    string PlayerId,
    string Email,
    string Password
    );