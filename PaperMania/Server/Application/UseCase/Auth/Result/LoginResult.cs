namespace Server.Application.UseCase.Auth.Result;

public record LoginResult(
    string SessionId,
    bool IsNewAccount);