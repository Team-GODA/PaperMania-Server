using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port;
using Server.Domain.Entity;

namespace Server.Infrastructure.Service;

public class DataService : IDataService
{
    private readonly IDataRepository _dataRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly ICurrencyRepository _currencyRepository;
    private readonly ISessionService _sessionService;
    private readonly IStageRepository _stageRepository;
    private readonly ILogger<DataService> _logger;

    public DataService(IDataRepository dataRepository, IAccountRepository accountRepository,
        ICurrencyRepository currencyRepository, ISessionService sessionService,
        IStageRepository stageRepository,
        ILogger<DataService> logger)
    {
        _dataRepository = dataRepository;
        _accountRepository = accountRepository;
        _currencyRepository = currencyRepository;
        _sessionService = sessionService;
        _stageRepository = stageRepository;
        _logger = logger;
    }
    
    public async Task<string> AddPlayerDataAsync(string playerName, string sessionId)
    {
        var existName = await _dataRepository.ExistsPlayerNameAsync(playerName);
        if (existName != null)
        {
            _logger.LogWarning($"이미 존재하는 이름입니다. player_name: {playerName}");
            throw new RequestException(ErrorStatusCode.Conflict, "PLAYER_NAME_EXIST",  new { PlayerName = playerName });
        }
        
        var userId = await _sessionService.GetUserIdBySessionIdAsync(sessionId);
        
        var isNewAccount = await _accountRepository.IsNewAccountAsync(userId);
        if (!isNewAccount)
        {
            _logger.LogWarning($"이미 등록된 계정입니다. player_name: { playerName }");
            throw new RequestException(ErrorStatusCode.Conflict, "PLAYER_DATA_EXIST",  new { PlayerName = playerName });
        }
        
        await _dataRepository.AddPlayerDataAsync(userId, playerName);
        await _stageRepository.CreatePlayerStageDataAsync(userId);
        await _currencyRepository.AddPlayerCurrencyDataByUserIdAsync(userId);
        await _accountRepository.UpdateIsNewAccountAsync(userId, false);
        
        return playerName;
    }

    public async Task<string?> GetPlayerNameByUserIdAsync(int? userId)
    {
        var data = await GetPlayerDataByIdAsync(userId);
        if (data == null)
            throw new RequestException(ErrorStatusCode.NotFound, "PLAYER_ACCOUNT_NOT_FOUND",  new { UserId = userId });

        return data.PlayerName;
    }

    private async Task<PlayerGameData?> GetPlayerDataByIdAsync(int? userId)
    {
        var data =  await _dataRepository.GetPlayerDataByIdAsync(userId);
        if (data == null)
            throw new RequestException(ErrorStatusCode.NotFound, "PLAYER_ACCOUNT_NOT_FOUND",  new { UserId = userId });
        
        return data;
    }

    public async Task<int> GetPlayerLevelByUserIdAsync(int? userId)
    {
        var data = await GetPlayerDataByUserId(userId);
        if (data == null)
            throw new RequestException(ErrorStatusCode.NotFound, "PLAYER_ACCOUNT_NOT_FOUND",  new { UserId = userId });
        
        return data.PlayerLevel;
    }

    public async Task<int> GetPlayerExpByUserIdAsync(int? userId)
    {
        var data = await GetPlayerDataByUserId(userId);
        if (data == null)
            throw new RequestException(ErrorStatusCode.NotFound, "PLAYER_ACCOUNT_NOT_FOUND",  new { UserId = userId });
        
        return data.PlayerExp;
    }

    public async Task<PlayerGameData> UpdatePlayerLevelByExpAsync(int? userId, int exp)
    {
        var data = await GetPlayerDataByUserId(userId);
        if (data == null)
            throw new RequestException(ErrorStatusCode.NotFound, "PLAYER_ACCOUNT_NOT_FOUND",  new { UserId = userId });
        
        data.PlayerExp += exp;

        while (true)
        {
            var levelData = await _dataRepository.GetLevelDataAsync(data.PlayerLevel);

            if (levelData == null || data.PlayerExp < levelData.MaxExp)
                break;

            data.PlayerExp -= levelData.MaxExp;
            data.PlayerLevel++;
        }

        await _dataRepository.UpdatePlayerLevelAsync(userId, data.PlayerLevel, data.PlayerExp);
        return data;
    }

    public async Task RenamePlayerNameAsync(int? userId, string? newPlayerName)
    {
        if (string.IsNullOrEmpty(newPlayerName))
            throw new RequestException(ErrorStatusCode.NotFound, "PLAYER_NEW_NAME_NOT_FOUND",  new { UserId = userId });
        
        var exists = await _dataRepository.ExistsPlayerNameAsync(newPlayerName);
        if (exists != null)
        {
            _logger.LogWarning($"이미 존재하는 이름입니다. player_name: {newPlayerName}");
            throw new RequestException(ErrorStatusCode.Conflict, "PLAYER_NAME_EXIST",  new { PlayerName = newPlayerName });
        }

        await _dataRepository.RenamePlayerNameAsync(userId, newPlayerName);
    }
    
    private async Task<PlayerGameData> GetPlayerDataByUserId(int? userId)
    {
        var data = await _dataRepository.GetPlayerDataByIdAsync(userId);
        if (data == null)
            throw new RequestException(ErrorStatusCode.NotFound, "PLAYER_ACCOUNT_NOT_FOUND",  new { UserId = userId });

        return data;
    }
}