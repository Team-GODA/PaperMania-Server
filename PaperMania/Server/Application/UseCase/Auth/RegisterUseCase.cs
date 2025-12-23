using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.Input.Auth;
using Server.Application.Port.Output.Persistence;
using Server.Application.UseCase.Auth.Command;
using Server.Domain.Entity;
using Server.Domain.Service;
using Server.Infrastructure.Persistence.Model;

namespace Server.Application.UseCase.Auth;

public class RegisterUseCase : IRegisterUseCase
{
    private readonly IAccountDao _dao;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUseCase(
        IAccountDao dao,
        IPasswordHasher passwordHasher
    )
    {
        _dao = dao;
        _passwordHasher = passwordHasher;
    }
    
    public async Task ExecuteAsync(RegisterCommand request)
    {
        request.Validate();

        var exists = await _dao.ExistsByPlayerIdAsync(request.PlayerId);
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
        
        await _dao.CreateAsync(newAccount);
    }
}