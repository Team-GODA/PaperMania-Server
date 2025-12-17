using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.In.Auth;
using Server.Application.Port.Out.Persistence;
using Server.Application.UseCase.Auth.Command;
using Server.Domain.Entity;
using Server.Domain.Service;

namespace Server.Application.UseCase.Auth;

public class RegisterUseCase : IRegisterUseCase
{
    private readonly IAccountRepository _repository;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUseCase(
        IAccountRepository repository,
        IPasswordHasher passwordHasher
    )
    {
        _repository = repository;
        _passwordHasher = passwordHasher;
    }
    
    public async Task ExecuteAsync(RegisterCommand request)
    {
        request.Validate();

        var exists = await _repository.ExistsByPlayerIdAsync(request.PlayerId);
        if (exists)
            throw new RequestException(
                ErrorStatusCode.Conflict,
                "PLAYER_ID_ALREADY_EXISTS"
            );

        var hashedPassword = _passwordHasher.Hash(request.Password);
        
        var newAccount = new PlayerAccountData
        {
            PlayerId = request.PlayerId,
            Email = request.Email,
            Password = hashedPassword,
            IsNewAccount = true,
            Role = "user"
        };
        
        await _repository.CreateAsync(newAccount);
    }
}