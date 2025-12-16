using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port;
using Server.Application.UseCase.Auth.Command;
using Server.Domain.Entity;
using Server.Domain.Service;

namespace Server.Application.UseCase.Auth;

public class RegisterUseCase
{
    private readonly IAccountRepository _repository;
    private readonly ISessionService _sessionService;
    private readonly UserService _userService;

    public RegisterUseCase(
        IAccountRepository repository,
        ISessionService sessionService,
        UserService userService
    )
    {
        _repository = repository;
        _sessionService = sessionService;
        _userService = userService;
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

        var hashedPassword = _userService.HashPassword(request.Password);
        
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