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
    private readonly IUnitOfWork _unitOfWork;
    
    public RegisterService(
        IAccountRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<RegisterResult?> ExecuteAsync(RegisterCommand request)
    {
        return await _unitOfWork.ExecuteAsync(async () =>
        {
            var existByEmail = await _repository.FindByEmailAsync(request.Email);
            if (existByEmail != null)
                throw new RequestException(ErrorStatusCode.Conflict,
                    "DUPLICATE_EMAIL", new { PlayerId = request.PlayerId, Email = existByEmail.Email });

            var existByPlayerId = await _repository.FindByPlayerIdAsync(request.PlayerId);
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
            if (createdPlayer == null)
                throw new RequestException(
                    ErrorStatusCode.ServerError,
                    "CREATE_ACCOUNT_FAILED");

            return new RegisterResult(
                Id: createdPlayer.Id
            );
        });
    }
}