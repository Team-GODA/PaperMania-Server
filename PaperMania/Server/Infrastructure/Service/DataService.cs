using Server.Application.Exceptions;
using Server.Application.Exceptions.Data;
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
            throw new PlayerNameExistException(playerName);
        }
        
        var userId = await _sessionService.GetUserIdBySessionIdAsync(sessionId);
        
        var isNewAccount = await _accountRepository.IsNewAccountAsync(userId);
        if (!isNewAccount)
        {
            _logger.LogWarning($"이미 등록된 계정입니다. player_name: {playerName}");
            throw new PlayerDataExistException();
        }
        
        await _dataRepository.AddPlayerDataAsync(userId, playerName);
        await _stageRepository.CreatePlayerStageDataAsync(userId);
        await _accountRepository.UpdateIsNewAccountAsync(userId, false);
        
        return playerName;
    }

    public async Task<string?> GetPlayerNameByUserIdAsync(int? userId)
    {
        var data = await GetPlayerDataByIdAsync(userId);
        if (data == null)
            throw new PlayerNotFoundException(userId);

        return data.PlayerName;
    }

    private async Task<PlayerGameData?> GetPlayerDataByIdAsync(int? userId)
    {
        return await _dataRepository.GetPlayerDataByIdAsync(userId);
    }

    public async Task<int> GetPlayerLevelByUserIdAsync(int? userId)
    {
        var data = await GetPlayerDataByUserId(userId);
        if (data == null)
            throw new PlayerNotFoundException(userId);
        
        return data.PlayerLevel;
    }

    public async Task<int> GetPlayerExpByUserIdAsync(int? userId)
    {
        var data = await GetPlayerDataByUserId(userId);
        if (data == null)
            throw new PlayerNotFoundException(userId);
        
        return data.PlayerExp;
    }

    public async Task<PlayerGameData> UpdatePlayerLevelByExpAsync(int? userId, int exp)
    {
        var playerData = await GetPlayerDataByUserId(userId);
        if (playerData == null)
            throw new PlayerNotFoundException(userId);
        
        playerData.PlayerExp += exp;

        while (true)
        {
            var levelData = await _dataRepository.GetLevelDataAsync(playerData.PlayerLevel);

            if (levelData == null || playerData.PlayerExp < levelData.MaxExp)
                break;

            playerData.PlayerExp -= levelData.MaxExp;
            playerData.PlayerLevel++;
        }

        await _dataRepository.UpdatePlayerLevelAsync(userId, playerData.PlayerLevel, playerData.PlayerExp);
        return playerData;
    }

    public async Task RenamePlayerNameAsync(int? userId, string? newPlayerName)
    {
        if (string.IsNullOrEmpty(newPlayerName))
            throw new PlayerNameMissingException();
        
        var exists = await _dataRepository.ExistsPlayerNameAsync(newPlayerName);
        if (exists != null)
        {
            _logger.LogWarning($"이미 존재하는 이름입니다. player_name: {newPlayerName}");
            throw new PlayerNameExistException(newPlayerName);
        }

        await _dataRepository.RenamePlayerNameAsync(userId, newPlayerName);
    }

    public async Task<PlayerCurrencyData> GetPlayerGoodsDataByUserIdAsync(int userId)
    {
        var data = await _currencyRepository.GetPlayerCurrencyDataByUserIdAsync(userId);
        return data;
    }

    private async Task<PlayerGameData> GetPlayerDataByUserId(int? userId)
    {
        var data = await _dataRepository.GetPlayerDataByIdAsync(userId);
        if (data == null)
            throw new Exception($"Id: {userId}의 플레이어 데이터가 없습니다.");

        return data;
    }
}