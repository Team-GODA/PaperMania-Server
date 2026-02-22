using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.Input.Auth;
using Server.Application.Port.Output.Persistence;
using Server.Application.Port.Output.Service;
using Server.Application.UseCase.Auth.Command;
using Server.Application.UseCase.Auth.Result;
using Server.Domain.Service;
using Server.Infrastructure.Cache;

namespace Server.Application.UseCase.Auth;

public class LoginUseCase : ILoginUseCase
{
    private readonly IAccountRepository _repository;
    private readonly ISessionService _sessionService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly CacheAsideService _cacheAsideService;

    public LoginUseCase(
        IAccountRepository repository,
        ISessionService sessionService,
        IPasswordHasher passwordHasher,
        CacheAsideService cacheAsideService
    )
    {
        _repository = repository;
        _sessionService = sessionService;
        _passwordHasher = passwordHasher;
        _cacheAsideService = cacheAsideService;
    }
    
    public async Task<LoginResult> ExecuteAsync(LoginCommand request)
    {
        request.Validate();
        
        var account = await _cacheAsideService.GetOrSetAsync(
            CacheKey.Account.ByPlayerId(request.PlayerId),
            async () => await _repository.FindByPlayerIdAsync(request.PlayerId),
            expiration: TimeSpan.FromDays(7)
        );
        
        if (account == null || string.IsNullOrEmpty(account.Password))
        {
            _passwordHasher.Verify(request.Password, "DUMMY_HASH");
            throw new RequestException(
                ErrorStatusCode.Unauthorized,
                "INVALID_CREDENTIALS");
        }

        if (!_passwordHasher.Verify(request.Password, account.Password))
            throw new RequestException(
                ErrorStatusCode.Unauthorized,
                "INVALID_PASSWORD");
        
        var sessionId = await _sessionService.CreateSessionAsync(account.Id);
        if (string.IsNullOrEmpty(sessionId))
            throw new RequestException(
                ErrorStatusCode.ServerError,
                "SESSION_CREATE_FAILED");
        
        return new LoginResult(
            sessionId,
            account.IsNewAccount
        );
    }
}