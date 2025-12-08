namespace Server.Application.UseCase.Auth.Command;

public record LoginCommand(
    string PlayerId, 
    string Password
    );