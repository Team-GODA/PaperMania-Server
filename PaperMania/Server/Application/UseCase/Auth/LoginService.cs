using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port;
using Server.Application.UseCase.Auth.Command;
using Server.Application.UseCase.Auth.Result;

namespace Server.Application.UseCase.Auth;

public class LoginService : ILoginUseCase
{
    private readonly IAccountRepository _accountRepository;
    private readonly ISessionService _sessionService;
    private readonly IUnitOfWork _unitOfWork;
    
    public LoginService(
        IAccountRepository accountRepository,
        ISessionService sessionService,
        IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _sessionService = sessionService;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<LoginResult> ExecuteAsync(LoginCommand request)
    {
        return await _unitOfWork.ExecuteAsync(async () =>
        {
            var account = await _accountRepository.FindByPlayerIdAsync(request.PlayerId);
            
            if (account == null)
                throw new RequestException(
                    ErrorStatusCode.NotFound, 
                    "USER_NOT_FOUND");
        
            if (string.IsNullOrEmpty(account.Password))
                throw new RequestException(ErrorStatusCode.ServerError, 
                    "NO_PASSWORD_DATA");
            
            var isVerified = BCrypt.Net.BCrypt.Verify(request.Password, account.Password);
            if (!isVerified)
                throw new RequestException(ErrorStatusCode.Unauthorized,
                    "PASSWORD_NOT_VERIFIED", new { PlayerId = request.PlayerId });

            var sessionId = await _sessionService.CreateSessionAsync(account.Id);
            if (string.IsNullOrEmpty(sessionId))
                throw new RequestException(ErrorStatusCode.ServerError,
                    "SESSION_CREATE_FAILED");

            return new LoginResult(
                SessionId: sessionId,
                IsNewAccount: account.IsNewAccount
            );
        });
    }
}