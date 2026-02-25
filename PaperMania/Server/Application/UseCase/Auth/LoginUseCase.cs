using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.Input.Auth;
using Server.Application.Port.Output.Cache;
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
    private readonly ICacheAsideService _cacheAsideService;

    public LoginUseCase(
        IAccountRepository repository,
        ISessionService sessionService,
        IPasswordHasher passwordHasher,
        ICacheAsideService cacheAsideService
    )
    {
        _repository = repository;
        _sessionService = sessionService;
        _passwordHasher = passwordHasher;
        _cacheAsideService = cacheAsideService;
    }
    
    public async Task<LoginResult> ExecuteAsync(LoginCommand request, CancellationToken ct)
    {
        request.Validate();
        
        var account = await _cacheAsideService.GetOrSetAsync(
            CacheKey.Account.ByPlayerId(request.PlayerId),
            async (token) => await _repository.FindByPlayerIdAsync(request.PlayerId, token),
            TimeSpan.FromDays(7),
            ct
        );
        
        if (account == null)
        {
            Console.WriteLine($"[DEBUG] Account not found for PlayerId: {request.PlayerId}");
        }
        else 
        {
            Console.WriteLine($"[DEBUG] Account found: {account.Id}, Hash: {account.Password?.Substring(0, 5)}...");
        }
        
        if (account == null || string.IsNullOrEmpty(account.Password))
        {
            _passwordHasher.Verify(request.Password, "$2a$11$12345678901234567890123456789012345678901234567890123");
        
            throw new RequestException(
                ErrorStatusCode.Unauthorized,
                "INVALID_CREDENTIALS");
        }

        Console.WriteLine($"[DEBUG] Comparing - Input: {request.Password}, Stored Hash: {account.Password}");
        
        account.VerifyPassword(request.Password, _passwordHasher);
        
        var sessionId = await _sessionService.CreateSessionAsync(account.Id, ct);
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