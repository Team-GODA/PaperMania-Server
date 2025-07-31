using Server.Application.Port;
using Server.Domain.Entity;

namespace Server.Infrastructure.Service;

public class CurrencyService : ICurrencyService
{
    private readonly ICurrencyRepository _currencyRepository;
    private readonly ISessionService _sessionService;
    private readonly ILogger<CurrencyService> _logger;

    public CurrencyService(ICurrencyRepository currencyRepository, ISessionService sessionService, ILogger<CurrencyService> logger)
    {
        _currencyRepository = currencyRepository;
        _sessionService = sessionService;
        _logger = logger;
    }

    private async Task ValidateSessionAsync(string sessionId)
    {
        var userId = await _sessionService.GetUserIdBySessionIdAsync(sessionId);

        var isValid = await _sessionService.ValidateSessionAsync(sessionId, userId);
        if (!isValid)
            throw new UnauthorizedAccessException("세션이 유효하지 않습니다.");
    }
    
    public async Task<int> GetPlayerActionPointAsync(int? userId, string sessionId)
    {
        await ValidateSessionAsync(sessionId);
        
        var data = await _currencyRepository.GetPlayerGoodsDataByUserIdAsync(userId);
        var updated = await RegenerateActionPointAsync(data);

        if (updated)
            _logger.LogInformation($"AP 자동 회복 적용: UserId={userId}, AP={data.ActionPoint}");

        return data.ActionPoint;
    }

    public async Task<int> UpdatePlayerMaxActionPoint(int? userId, int newMaxActionPoint, string sessionId)
    {
        await ValidateSessionAsync(sessionId);
        
        var data = await _currencyRepository.GetPlayerGoodsDataByUserIdAsync(userId);
        data.MaxActionPoint = newMaxActionPoint;

        await _currencyRepository.UpdatePlayerGoodsDataAsync(data);
        return newMaxActionPoint;
    }

    public async Task UsePlayerActionPointAsync(int? userId, int usedActionPoint, string sessionId)
    {
        await ValidateSessionAsync(sessionId);

        var data = await _currencyRepository.GetPlayerGoodsDataByUserIdAsync(userId);
        await RegenerateActionPointAsync(data);

        data.ActionPoint = Math.Max(data.ActionPoint - usedActionPoint, 0);
        data.LastActionPointUpdated = DateTime.UtcNow;

        await _currencyRepository.UpdatePlayerGoodsDataAsync(data);
    }

    public async Task<int> GetPlayerGoldAsync(int? userId, string sessionId)
    {
        await ValidateSessionAsync(sessionId);
        
        var data = await _currencyRepository.GetPlayerGoodsDataByUserIdAsync(userId);
        return data.Gold;
    }

    public async Task AddPlayerGoldAsync(int? userId, int gold, string sessionId)
    {
        await ValidateSessionAsync(sessionId);
        
        var data = await _currencyRepository.GetPlayerGoodsDataByUserIdAsync(userId);
        data.Gold += gold;
        
        await _currencyRepository.UpdatePlayerGoodsDataAsync(data);
    }

    public async Task UsePlayerGoldAsync(int? userId, int usedGold, string sessionId)
    {
        await ValidateSessionAsync(sessionId);
        
        var data = await _currencyRepository.GetPlayerGoodsDataByUserIdAsync(userId);
        data.Gold = Math.Max(data.Gold - usedGold, 0);
        
        await _currencyRepository.UpdatePlayerGoodsDataAsync(data);
    }

    public async Task<int> GetPlayerPaperPieceAsync(int? userId, string sessionId)
    {
        await ValidateSessionAsync(sessionId);
        ;
        var data = await _currencyRepository.GetPlayerGoodsDataByUserIdAsync(userId);
        return data.PaperPiece;
    }

    public async Task AddPlayerPaperPieceAsync(int? userId, int paperPiece, string sessionId)
    {
        await ValidateSessionAsync(sessionId);
        
        var data = await _currencyRepository.GetPlayerGoodsDataByUserIdAsync(userId);
        data.PaperPiece += paperPiece;
        
        await _currencyRepository.UpdatePlayerGoodsDataAsync(data);
    }

    public async Task UsePlayerPaperPieceAsync(int? userId, int usedPaperPiece, string sessionId)
    {
        await ValidateSessionAsync(sessionId);
        
        var data = await _currencyRepository.GetPlayerGoodsDataByUserIdAsync(userId);
        data.PaperPiece = Math.Max(data.PaperPiece - usedPaperPiece, 0);
        
        await _currencyRepository.UpdatePlayerGoodsDataAsync(data);
    }

    private async Task<bool> RegenerateActionPointAsync(PlayerCurrencyData data)
    {
        var currentActionPoint = data.ActionPoint;
        var maxActionPoint = data.MaxActionPoint;
        var lastRegenTime = data.LastActionPointUpdated;

        var nowUtc = DateTime.UtcNow;

        int regenIntervalSeconds = 240;
        int secondsPassed = (int)(nowUtc - lastRegenTime).TotalSeconds;
        int regenAmount = secondsPassed / regenIntervalSeconds;

        _logger.LogInformation($"현재 AP : {currentActionPoint} / MaxAP: {maxActionPoint}");

        if (regenAmount > 0 && currentActionPoint < maxActionPoint)
        {
            int apToAdd = Math.Min(regenAmount, maxActionPoint - currentActionPoint);
            currentActionPoint += apToAdd;
            data.LastActionPointUpdated = lastRegenTime.AddSeconds(apToAdd * regenIntervalSeconds);
            data.ActionPoint = currentActionPoint;

            _logger.LogInformation($"AP 증가: {apToAdd}, 새 AP: {currentActionPoint}");
            _logger.LogInformation($"LastActionPointUpdated 갱신: {data.LastActionPointUpdated}");

            await _currencyRepository.UpdatePlayerGoodsDataAsync(data);
            return true;
        }

        return false;
    }
}