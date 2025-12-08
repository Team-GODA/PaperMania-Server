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

    public AddPlayerService(
        IDataRepository  dataRepository,
        IAccountRepository  accountRepository,
        ICurrencyRepository currencyRepository,
        ISessionService sessionService,
        IStageRepository stageRepository)
    {
        _dataRepository = dataRepository;
        _accountRepository = accountRepository;
        _currencyRepository = currencyRepository;
        _sessionService = sessionService;
        _stageRepository = stageRepository;
    }
    
    public async Task ExecuteAsync(AddPlayerDataCommand request)
    {
        var existName = await _dataRepository.ExistsPlayerNameAsync(request.PlayerName);
        if (existName != null)
            throw new RequestException(ErrorStatusCode.Conflict, 
                "PLAYER_NAME_EXIST",  new { PlayerName = request.PlayerName });
        
        var userId = await _sessionService.GetUserIdBySessionIdAsync(request.SessionId);
        
        var isNewAccount = await _accountRepository.IsNewAccountAsync(userId);
        if (!isNewAccount)
            throw new RequestException(ErrorStatusCode.Conflict, 
                "PLAYER_DATA_EXIST",  new { PlayerName = request.PlayerName });

        await Task.WhenAll(
            _dataRepository.AddPlayerDataAsync(userId, request.PlayerName),
            _currencyRepository.AddPlayerCurrencyDataByUserIdAsync(userId),
            _stageRepository.CreatePlayerStageDataAsync(userId),
            _accountRepository.UpdateIsNewAccountAsync(userId, false)
        );
    }
}