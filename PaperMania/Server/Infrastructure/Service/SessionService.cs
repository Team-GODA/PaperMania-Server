using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port;
using Server.Infrastructure.Cache;

namespace Server.Infrastructure.Service;

public class SessionService : ISessionService
{
    private readonly ICacheService _cacheService;
    private readonly ILogger<SessionService> _logger;
    private readonly TimeSpan _sessionTimeout = TimeSpan.FromHours(24);

    public SessionService(ICacheService cacheService, 
        ILogger<SessionService> logger)
    {
        _cacheService = cacheService;
        _logger = logger;
    }
    
    public async Task<string> CreateSessionAsync(int userId)
    {
        var sessionId = GenerateSessionId();
        
        _logger.LogInformation($"세션 생성: UserId={userId}, SessionId={sessionId}");

        await _cacheService.SetAsync(CacheKey.Session.ById(sessionId), userId.ToString(), _sessionTimeout);
        
        return sessionId;
    }
    
    private string GenerateSessionId()
        => Guid.NewGuid().ToString();

    public async Task<bool> ValidateSessionAsync(string sessionId, int? userId = null)
    {
        var exists = await _cacheService.ExistsAsync(CacheKey.Session.ById(sessionId));
        if (!exists)
        {
            _logger.LogWarning($"세션 존재하지 않음: SessionId={sessionId}");
            return false;
        }

        if (userId.HasValue)
        {
            var storedUserId = await GetUserIdBySessionIdAsync(sessionId);
            if (storedUserId != userId)
            {
                _logger.LogWarning($"유저 검증 실패: UserId ; {storedUserId} != {userId}");
                return false;
            }
        }
        
        return true;
    }

    public async Task RefreshSessionAsync(string sessionId)
    {
        var userId = await GetUserIdBySessionIdAsync(sessionId);
        
        await _cacheService.SetExpirationAsync(CacheKey.Session.ById(sessionId), _sessionTimeout);
        await _cacheService.SetExpirationAsync(CacheKey.SessionByUserId(userId), _sessionTimeout);
        
        _logger.LogInformation($"세션 TTL 갱신: SessionId={sessionId}, UserId={userId}");
    }

    public async Task<int> GetUserIdBySessionIdAsync(string sessionId)
    {
        var value = await _cacheService.GetAsync(CacheKey.Session(sessionId));
        if (value != null && int.TryParse(value, out var userId))
            return userId;

        _logger.LogWarning($"세션 아이디로 유저 조회 실패: SessionId={sessionId}");
        throw new RequestException(
            ErrorStatusCode.NotFound,
            "USER_NOT_FOUND_BY_SESSION_ID",
            new { SessionId = sessionId }
        );
    }

    public async Task DeleteSessionAsync(string sessionId)
    {
        _logger.LogInformation($"[DeleteSessionAsync] 세션 삭제 요청: SessionId={sessionId}");
        
        await _cacheService.RemoveAsync(CacheKey.Session(sessionId));
        
        _logger.LogInformation($"[DeleteSessionAsync] 세션 삭제 완료: SessionId={sessionId}");
    }
}