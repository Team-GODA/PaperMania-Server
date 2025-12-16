using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port;
using Server.Application.UseCase.Auth.Command;
using Server.Application.UseCase.Auth.Result;
using Server.Domain.Entity;

namespace Server.Application.UseCase.Auth;

public class RegisterService : IRegisterUseCase
{
    private readonly IAccountRepository _repository;
    private readonly ITransactionScope _transactionScope;

    public RegisterService(
        IAccountRepository repository,
        ITransactionScope transactionScope)
    {
        _repository = repository;
        _transactionScope = transactionScope;
    }

    public async Task<RegisterResult?> ExecuteAsync(RegisterCommand request)
    {
        if (string.IsNullOrWhiteSpace(request.PlayerId))
            throw new RequestException(ErrorStatusCode.BadRequest, "INVALID_PLAYER_ID");

        if (string.IsNullOrWhiteSpace(request.Email))
            throw new RequestException(ErrorStatusCode.BadRequest, "INVALID_EMAIL");

        if (!request.Email.Contains("@"))
            throw new RequestException(ErrorStatusCode.BadRequest, "INVALID_EMAIL_FORMAT");

        if (string.IsNullOrWhiteSpace(request.Password))
            throw new RequestException(ErrorStatusCode.BadRequest, "INVALID_PASSWORD");


        return await _transactionScope.ExecuteAsync(async () =>
        {
            var checkTasks = await Task.WhenAll(
                _repository.FindByEmailAsync(request.Email),
                _repository.FindByPlayerIdAsync(request.PlayerId)
            );

            var existByEmail = checkTasks[0];
            var existByPlayerId = checkTasks[1];

            if (existByEmail != null)
                throw new RequestException(ErrorStatusCode.Conflict, "DUPLICATE_EMAIL");

            if (existByPlayerId != null)
                throw new RequestException(ErrorStatusCode.Conflict, "DUPLICATE_PLAYER_ID");

            var newAccount = new PlayerAccountData
            {
                PlayerId = request.PlayerId,
                Email = request.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password, workFactor: 10),
                IsNewAccount = true,
                Role = "user"
            };

            var createdPlayer = await _repository.CreateAccountAsync(newAccount);

            if (createdPlayer == null)
                throw new RequestException(ErrorStatusCode.ServerError, "CREATE_ACCOUNT_FAILED");

            return new RegisterResult(createdPlayer.Id);
        });
    }
}