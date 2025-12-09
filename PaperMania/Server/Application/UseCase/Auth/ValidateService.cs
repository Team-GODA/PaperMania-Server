using Server.Application.Port;
using Server.Application.UseCase.Auth.Command;
using Server.Application.UseCase.Auth.Result;

namespace Server.Application.UseCase.Auth;

public class ValidateService : IValidateUseCase
{
    private readonly ISessionService _sessionService;
    private readonly IUnitOfWork _unitOfWork;

    public ValidateService(
        ISessionService sessionService,
        IUnitOfWork unitOfWork)
    {
        _sessionService = sessionService;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<ValidateResult> ExecuteAsync(ValidateCommand request)
    {
        return await _unitOfWork.ExecuteAsync(async () =>
        {
            var userId = await _sessionService.FindUserIdBySessionIdAsync(request.SessionId);

            return new ValidateResult(
                UserId: userId,
                IsValidated: true
            );
        });
    }
}