using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port;
using Server.Application.UseCase.Auth.Command;

namespace Server.Application.UseCase.Auth;

public class LogoutService : ILogoutUseCase
{
    private readonly ISessionService _sessionService;
    private readonly IUnitOfWork _unitOfWork;

    public LogoutService(
        ISessionService sessionService,
        IUnitOfWork unitOfWork)
    {
        _sessionService = sessionService;
        _unitOfWork = unitOfWork;
    }
    
    public async Task ExecuteAsync(LogoutCommand request)
    {
        if (string.IsNullOrWhiteSpace(request.SessionId))
            throw new RequestException(ErrorStatusCode.BadRequest,
                "INVALID_SESSION_ID");
        
        await _unitOfWork.ExecuteAsync(async () =>
        {
            await _sessionService.DeleteSessionAsync(request.SessionId);
        });
    }
}