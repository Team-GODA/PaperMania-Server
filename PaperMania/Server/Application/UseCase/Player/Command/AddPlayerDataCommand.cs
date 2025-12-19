namespace Server.Application.UseCase.Player.Command;

public record AddPlayerDataCommand(
    string PlayerName,
    string SessionId
    );