using Server.Api.Dto.Response;
using Server.Application.Exceptions;
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
        if (data == null)
            throw new RequestException(ErrorStatusCode.NotFound, "PLAYER_CURRENCY_DATA_NOT_FOUND",  new { UserId = userId });
        
        var updated = await RegenerateActionPointAsync(data);
        if (updated)
            _logger.LogInformation($"AP 자동 회복 적용: UserId={userId}, AP={data.ActionPoint}");

        return data.ActionPoint;
    }

    public async Task<int> UpdatePlayerMaxActionPoint(int? userId, int newMaxActionPoint)
    {
        var data = await GetPlayerCurrencyDataOrException(userId);
        
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

    public async Task ModifyPlayerGoldAsync(int? userId, int amount)
    {
        var data = await GetPlayerCurrencyDataOrException(userId);

        if (amount >= 0)
        {
            _logger.LogInformation($"플레이어 골드 추가 성공 :  {userId}, Amount : {amount}");
            data.Gold += amount;
        }
        else
        {
            var decrease = -amount;
            
            if (data.Gold < decrease)
            {
                _logger.LogWarning($"골드 부족: UserId={userId}, 현재={data.Gold}, 요청={decrease}");
                throw new RequestException(ErrorStatusCode.Conflict, "NOT_ENOUGH_GOLD", new { UserId =  userId });
            }
            
            _logger.LogInformation($"플레이어 골드 사용 : UserId={userId}, Amount={decrease}");
            data.Gold -= decrease;
        }
        
        await _currencyRepository.UpdatePlayerCurrencyDataAsync(data);
    }

    public async Task<int> GetPlayerPaperPieceAsync(int? userId)
    {
        var data = await GetPlayerCurrencyDataOrException(userId);
        return data.PaperPiece;
    }

    public async Task ModifyPlayerPaperPieceAsync(int? userId, int amount)
    {
        var data = await GetPlayerCurrencyDataOrException(userId);

        if (amount >= 0)
        {
            _logger.LogInformation($"플레이어 종이 조각 추가 성공 :  {userId}, Amount : {amount}");
            data.PaperPiece += amount;
        }
        else
        {
            var decrease = -amount;
            
            if (data.PaperPiece < decrease)
            {
                _logger.LogWarning($"종이 조각 부족: UserId={userId}, 현재={data.PaperPiece}, 요청={decrease}");
                throw new RequestException(ErrorStatusCode.Conflict, "NOT_ENOUGH_PAPER_PIECE", new { UserId =  userId });
            }
            
            _logger.LogInformation($"플레이어 종이 조각 사용 : UserId={userId}, Amount={decrease}");
            data.PaperPiece -= decrease;
        }
        
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
        var data = await _currencyRepository.GetPlayerCurrencyDataByUserIdAsync(userId);
        if (data == null)
            throw new RequestException(ErrorStatusCode.NotFound, "PLAYER_CURRENCY_DATA_NOT_FOUND",  new { UserId = userId });

        return data;
    }
}