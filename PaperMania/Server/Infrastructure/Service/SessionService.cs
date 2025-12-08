using Server.Api.Dto.Response;
using Server.Application.Configure;
using Server.Application.Exceptions;
using Server.Application.Port;
using Server.Infrastructure.Cache;

namespace Server.Infrastructure.Service;

public class SessionService : ISessionService
{
    private readonly ICacheService _cacheService;
    private readonly ILogger<SessionService> _logger;

    public SessionService(ICacheService cacheService, 
        ILogger<SessionService> logger)
    {
        _cacheService = cacheService;
        _logger = logger;
    }
    
    public async Task<string> CreateSessionAsync(int userId)
    {
        var existingSessionId = await _cacheService.GetAsync(CacheKey.Session.ByUserId(userId));
        if (existingSessionId != null)
        {
            await Task.WhenAll(
                _cacheService.DeleteAsync(CacheKey.Session.BySessionId(existingSessionId)),
                _cacheService.DeleteAsync(CacheKey.Session.ByUserId(userId))
            );
            _logger.LogInformation($"기존 세션 삭제: UserId={userId}, SessionId={existingSessionId}");
        }
        
        var sessionId = GenerateSessionId();
        
        _logger.LogInformation($"세션 생성: UserId={userId}, SessionId={sessionId}");

        await Task.WhenAll(
            _cacheService.SetAsync(
                CacheKey.Session.BySessionId(sessionId), userId.ToString(), CacheTTLConfig.Session.Timeout),
            _cacheService.SetAsync(
                CacheKey.Session.ByUserId(userId), sessionId, CacheTTLConfig.Session.Timeout)
            );
        
        return sessionId;
    }
    
    private string GenerateSessionId()
        => Guid.NewGuid().ToString();

    public async Task<bool> ValidateSessionAsync(string sessionId)
    {
        var value = await _cacheService.GetAsync(CacheKey.Session.BySessionId(sessionId));
        return !string.IsNullOrEmpty(value);
    }

    public async Task RefreshSessionAsync(string sessionId)
    {
        var userId = await GetUserIdBySessionIdAsync(sessionId);

        await Task.WhenAll(
            _cacheService.SetExpirationAsync(
                CacheKey.Session.BySessionId(sessionId), CacheTTLConfig.Session.Timeout),
            _cacheService.SetExpirationAsync(
                CacheKey.Session.ByUserId(userId), CacheTTLConfig.Session.Timeout));
        
        _logger.LogInformation($"세션 TTL 갱신: UserId={userId}");
    }

    public async Task<int> GetUserIdBySessionIdAsync(string sessionId)
    {
        if (!await ValidateSessionAsync(sessionId))
        {
            _logger.LogWarning($"세션 검증 실패: SessionId={sessionId}");
            throw new RequestException(
                ErrorStatusCode.Unauthorized,
                "INVALID_SESSION");
        }
        
        var userIdStr = await _cacheService.GetAsync(CacheKey.Session.BySessionId(sessionId));
        if (userIdStr != null && int.TryParse(userIdStr, out var userId))
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
        _logger.LogInformation($"세션 삭제 요청: SessionId={sessionId}");
        
        var userIdStr = await _cacheService.GetAsync(CacheKey.Session.BySessionId(sessionId));
    
        if (userIdStr != null && int.TryParse(userIdStr, out var userId))
        {
            await Task.WhenAll(
                _cacheService.DeleteAsync(CacheKey.Session.BySessionId(sessionId)),
                _cacheService.DeleteAsync(CacheKey.Session.ByUserId(userId))
            );
        
            _logger.LogInformation($"세션 삭제 완료: SessionId={sessionId}, UserId={userId}");
        }
        else
        {
            await _cacheService.DeleteAsync(CacheKey.Session.BySessionId(sessionId));
            _logger.LogWarning($"세션 삭제 (UserId 없음): SessionId={sessionId}");
        }
    }
}