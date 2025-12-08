namespace Server.Application.UseCase.Data.Command;

public record AddPlayerDataCommand(
    string PlayerName,
    string SessionId
    );