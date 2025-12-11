using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port;
using Server.Application.UseCase.Data.Command;

namespace Server.Application.UseCase.Data;

public class AddPlayerService : IAddPlayerDataUseCase
{
    private readonly IDataRepository _dataRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly ICurrencyRepository _currencyRepository;
    private readonly ISessionService _sessionService;
    private readonly IStageRepository _stageRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddPlayerService(
        IDataRepository  dataRepository,
        IAccountRepository  accountRepository,
        ICurrencyRepository currencyRepository,
        ISessionService sessionService,
        IStageRepository stageRepository,
        IUnitOfWork unitOfWork)
    {
        _dataRepository = dataRepository;
        _accountRepository = accountRepository;
        _currencyRepository = currencyRepository;
        _sessionService = sessionService;
        _stageRepository = stageRepository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task ExecuteAsync(AddPlayerDataCommand request)
    {
        var existName = await _dataRepository.ExistsPlayerNameAsync(request.PlayerName);
        if (existName != null)
            throw new RequestException(
                ErrorStatusCode.Conflict,
                "PLAYER_NAME_EXIST");

        var userId = await _sessionService.FindUserIdBySessionIdAsync(request.SessionId);
        if (userId == null)
            throw new RequestException(
                ErrorStatusCode.Unauthorized,
                "SESSION_DATA_CORRUPTED");

        var isNewAccount = await _accountRepository.IsNewAccountAsync(userId);
        if (isNewAccount == null)
            throw new RequestException(
                ErrorStatusCode.NotFound,
                "ACCOUNT_NOT_FOUND");

        if (!isNewAccount.Value)
            throw new RequestException(
                ErrorStatusCode.Conflict,
                "PLAYER_DATA_EXIST");

    
        await _unitOfWork.ExecuteAsync(async () =>
        {
            await _dataRepository.AddPlayerDataAsync(userId, request.PlayerName);
            await _currencyRepository.AddPlayerCurrencyDataByUserIdAsync(userId);
            await _stageRepository.CreatePlayerStageDataAsync(userId);
            await _accountRepository.UpdateIsNewAccountAsync(userId, false);
        });
    }
}