using Server.Application.Exceptions.Currency;
using Server.Application.Exceptions.Data;
using Server.Application.Port;
using Server.Domain.Entity;

namespace Server.Infrastructure.Service;

public class CurrencyService : ICurrencyService
{
    private readonly ICurrencyRepository _currencyRepository;
    private readonly ILogger<CurrencyService> _logger;

    public CurrencyService(ICurrencyRepository currencyRepository, ILogger<CurrencyService> logger)
    {
        _currencyRepository = currencyRepository;
        _logger = logger;
    }
    
    public async Task<int> GetPlayerActionPointAsync(int? userId)
    {
        var data = await GetPlayerCurrencyDataOrException(userId);
        
        var updated = await RegenerateActionPointAsync(data);
        if (updated)
            _logger.LogInformation($"AP 자동 회복 적용: UserId={userId}, AP={data.ActionPoint}");

        return data.ActionPoint;
    }

    public async Task<int> UpdatePlayerMaxActionPoint(int? userId, int newMaxActionPoint)
    {
        var data = await _currencyRepository.GetPlayerCurrencyDataByUserIdAsync(userId);
        if (data == null)
            throw new CurrencyDataNotFoundException(userId);
        
        data.MaxActionPoint = newMaxActionPoint;

        await _currencyRepository.UpdatePlayerCurrencyDataAsync(data);
        return newMaxActionPoint;
    }

    public async Task UsePlayerActionPointAsync(int? userId, int usedActionPoint)
    {
        var data = await GetPlayerCurrencyDataOrException(userId);
        await RegenerateActionPointAsync(data);

        data.ActionPoint = Math.Max(data.ActionPoint - usedActionPoint, 0);
        data.LastActionPointUpdated = DateTime.UtcNow;

        await _currencyRepository.UpdatePlayerCurrencyDataAsync(data);
    }

    public async Task<int> GetPlayerGoldAsync(int? userId)
    {
        var data = await GetPlayerCurrencyDataOrException(userId);
        return data.Gold;
    }

    public async Task AddPlayerGoldAsync(int? userId, int gold)
    {
        var data = await GetPlayerCurrencyDataOrException(userId);
        data.Gold += gold;
        
        await _currencyRepository.UpdatePlayerCurrencyDataAsync(data);
    }

    public async Task UsePlayerGoldAsync(int? userId, int usedGold)
    {
        var data = await GetPlayerCurrencyDataOrException(userId);
        data.Gold = Math.Max(data.Gold - usedGold, 0);
        
        await _currencyRepository.UpdatePlayerCurrencyDataAsync(data);
    }

    public async Task<int> GetPlayerPaperPieceAsync(int? userId)
    {
        var data = await GetPlayerCurrencyDataOrException(userId);
        return data.PaperPiece;
    }

    public async Task AddPlayerPaperPieceAsync(int? userId, int paperPiece)
    {
        var data = await GetPlayerCurrencyDataOrException(userId);
        data.PaperPiece += paperPiece;
        
        await _currencyRepository.UpdatePlayerCurrencyDataAsync(data);
    }

    public async Task UsePlayerPaperPieceAsync(int? userId, int usedPaperPiece)
    {
        var data = await GetPlayerCurrencyDataOrException(userId);
        data.PaperPiece = Math.Max(data.PaperPiece - usedPaperPiece, 0);
        
        await _currencyRepository.UpdatePlayerCurrencyDataAsync(data);
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

            await _currencyRepository.UpdatePlayerCurrencyDataAsync(data);
            return true;
        }

        return false;
    }

    private async Task<PlayerCurrencyData> GetPlayerCurrencyDataOrException(int? userId)
    {
        if (userId == null)
            throw new UserIdNotFoundException(userId);

        var data = await _currencyRepository.GetPlayerCurrencyDataByUserIdAsync(userId);
        if (data == null)
            throw new CurrencyDataNotFoundException(userId);

        return data;
    }
}