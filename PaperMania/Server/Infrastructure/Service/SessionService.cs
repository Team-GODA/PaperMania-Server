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

    public SessionService(
        ICacheService cacheService, 
        ILogger<SessionService> logger)
    {
        _cacheService = cacheService;
        _logger = logger;
    }
    
    public async Task<string> CreateSessionAsync(int userId)
    {
        await CleanupExistingSessionAsync(userId);
        
        var sessionId = GenerateSessionId();
        
        await Task.WhenAll(
            _cacheService.SetAsync(
                CacheKey.Session.BySessionId(sessionId), 
                userId.ToString(), 
                CacheTTLConfig.Session.Timeout),
            _cacheService.SetAsync(
                CacheKey.Session.ByUserId(userId), 
                sessionId, 
                CacheTTLConfig.Session.Timeout)
        );
        
        _logger.LogInformation(
            "세션 생성 완료. UserId={UserId}, SessionId={SessionId}", 
            userId, 
            sessionId);
        
        return sessionId;
    }
    
    private async Task CleanupExistingSessionAsync(int userId)
    {
        var existingSessionId = await _cacheService.GetAsync(
            CacheKey.Session.ByUserId(userId));
        
        if (existingSessionId == null)
            return;
        
        await Task.WhenAll(
            _cacheService.DeleteAsync(CacheKey.Session.BySessionId(existingSessionId)),
            _cacheService.DeleteAsync(CacheKey.Session.ByUserId(userId))
        );
        
        _logger.LogInformation(
            "기존 세션 삭제 완료. UserId={UserId}, OldSessionId={SessionId}", 
            userId, 
            existingSessionId);
    }
    
    private static string GenerateSessionId()
        => Guid.NewGuid().ToString("N");

    public async Task<bool> ValidateSessionAsync(string sessionId)
    {
        var value = await _cacheService.GetAsync(
            CacheKey.Session.BySessionId(sessionId));
        return !string.IsNullOrEmpty(value);
    }

    public async Task RefreshSessionAsync(string sessionId)
    {
        var userId = await FindUserIdBySessionIdAsync(sessionId);

        await Task.WhenAll(
            _cacheService.SetExpirationAsync(
                CacheKey.Session.BySessionId(sessionId), 
                CacheTTLConfig.Session.Timeout),
            _cacheService.SetExpirationAsync(
                CacheKey.Session.ByUserId(userId), 
                CacheTTLConfig.Session.Timeout)
        );
        
        _logger.LogInformation(
            "세션 TTL 갱신 완료. UserId={UserId}, SessionId={SessionId}", 
            userId, 
            sessionId);
    }

    public async Task<int> FindUserIdBySessionIdAsync(string sessionId)
    {
        var userIdStr = await _cacheService.GetAsync(
            CacheKey.Session.BySessionId(sessionId));
    
        if (string.IsNullOrEmpty(userIdStr))
        {
            _logger.LogWarning(
                "유효하지 않거나 만료된 세션. SessionId={SessionId}", 
                sessionId);
            throw new RequestException(
                ErrorStatusCode.Unauthorized,
                "INVALID_SESSION");
        }
    
        if (!int.TryParse(userIdStr, out var userId))
        {
            _logger.LogError(
                "세션 데이터 손상. SessionId={SessionId}, Value={Value}", 
                sessionId, 
                userIdStr);
            
            await DeleteSessionAsync(sessionId);
            
            throw new RequestException(
                ErrorStatusCode.ServerError,
                "SESSION_DATA_CORRUPTED");
        }
    
        return userId;
    }

    public async Task DeleteSessionAsync(string sessionId)
    {
        var userIdStr = await _cacheService.GetAsync(
            CacheKey.Session.BySessionId(sessionId));
    
        if (string.IsNullOrEmpty(userIdStr))
        {
            _logger.LogDebug(
                "이미 삭제되었거나 존재하지 않는 세션. SessionId={SessionId}", 
                sessionId);
            return;
        }
        
        if (int.TryParse(userIdStr, out var userId))
        {
            await Task.WhenAll(
                _cacheService.DeleteAsync(CacheKey.Session.BySessionId(sessionId)),
                _cacheService.DeleteAsync(CacheKey.Session.ByUserId(userId))
            );
        
            _logger.LogInformation(
                "세션 삭제 완료. SessionId={SessionId}, UserId={UserId}", 
                sessionId, 
                userId);
        }
        else
        {
            await _cacheService.DeleteAsync(CacheKey.Session.BySessionId(sessionId));
            
            _logger.LogWarning(
                "유효하지 않은 userId 데이터를 가진 세션 삭제. SessionId={SessionId}, Value={Value}", 
                sessionId, 
                userIdStr);
        }
    }
}