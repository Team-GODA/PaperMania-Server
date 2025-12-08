namespace Server.Application.UseCase.Auth.Result;

public record ValidateResult(
    int UserId,
    bool IsValidated
    );