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
    
    public RegisterService(IAccountRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<RegisterResult?> ExecuteAsync(RegisterCommand request)
    {
        var existByEmail = await _repository.GetAccountDataByEmailAsync(request.Email);
        if (existByEmail != null)
            throw new RequestException(ErrorStatusCode.Conflict, 
                "DUPLICATE_EMAIL", new { PlayerId = request.PlayerId, Email = existByEmail.Email });

        var existByPlayerId = await _repository.GetAccountDataByPlayerIdAsync(request.PlayerId);
        if (existByPlayerId != null)
            throw new RequestException(ErrorStatusCode.Conflict, 
                "DUPLICATE_PLAYER_ID", new { PlayerId = request.PlayerId });

        var newAccount = new PlayerAccountData
        {
            PlayerId = request.PlayerId,
            Email = request.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
            IsNewAccount = true,
            Role = "user"
        };
        
        var createdPlayer = await _repository.AddAccountAsync(newAccount);

        return new RegisterResult(
            Id:createdPlayer.Id
            );
    }
}