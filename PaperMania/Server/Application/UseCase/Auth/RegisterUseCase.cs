using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port;
using Server.Application.UseCase.Auth.Command;
using Server.Application.UseCase.Auth.Result;
using Server.Domain.Entity;
using Server.Domain.Service;

namespace Server.Application.UseCase.Auth;

public class RegisterUseCase : IRegisterUseCase
{
    private readonly IAccountRepository _repository;
    private readonly UserService _userService;
    private readonly ITransactionScope _transactionScope;

    public RegisterUseCase(
        IAccountRepository repository,
        UserService userService,
        ITransactionScope transactionScope)
    {
        _repository = repository;
        _userService = userService;
        _transactionScope = transactionScope;
    }

    public async Task<RegisterResult?> ExecuteAsync(RegisterCommand request)
    {
        ValidateInput(request);
        
        return await _transactionScope.ExecuteAsync(async () =>
        {
            await ValidateDuplicatesAsync(request.Email, request.PlayerId);
                
            var newAccount = new PlayerAccountData
            {
                PlayerId = request.PlayerId,
                Email = request.Email,
                Password = _userService.HashPassword(request.Password),
                IsNewAccount = true,
                Role = "user"
            };

            var createdAccount = await _repository.CreateAccountAsync(newAccount);
            if (createdAccount == null)
                throw new RequestException(
                    ErrorStatusCode.ServerError, 
                    "CREATE_ACCOUNT_FAILED");

            return new RegisterResult(
                createdAccount.Id
                );
        });
    }

    private static void ValidateInput(RegisterCommand request)
    {
        if (string.IsNullOrWhiteSpace(request.PlayerId))
            throw new RequestException(
                ErrorStatusCode.BadRequest,
                "INVALID_PLAYER_ID");

        if (string.IsNullOrWhiteSpace(request.Email))
            throw new RequestException(
                ErrorStatusCode.BadRequest, 
                "INVALID_EMAIL");

        if (!request.Email.Contains("@"))
            throw new RequestException(
                ErrorStatusCode.BadRequest,
                "INVALID_EMAIL_FORMAT");

        if (string.IsNullOrWhiteSpace(request.Password))
            throw new RequestException(
                ErrorStatusCode.BadRequest,
                "INVALID_PASSWORD");
        
        if (request.Password.Length < 8)
            throw new RequestException(
                ErrorStatusCode.BadRequest, 
                "PASSWORD_TOO_SHORT");
    }
    
    private async Task ValidateDuplicatesAsync(string email, string playerId)
    {
        var (existByEmail, existByPlayerId) = await Task.WhenAll(
            _repository.FindByEmailAsync(email),
            _repository.FindByPlayerIdAsync(playerId)
        ).ContinueWith(t => (t.Result[0], t.Result[1]));

        if (existByEmail != null)
            throw new RequestException(
                ErrorStatusCode.Conflict, 
                "DUPLICATE_EMAIL");

        if (existByPlayerId != null)
            throw new RequestException(
                ErrorStatusCode.Conflict, 
                "DUPLICATE_PLAYER_ID");
    }
}