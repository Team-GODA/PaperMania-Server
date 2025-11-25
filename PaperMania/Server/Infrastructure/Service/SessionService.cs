using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port;

namespace Server.Infrastructure.Service;

public class SessionService : ISessionService
{
    private readonly ICacheService _cacheService;
    private readonly ILogger<SessionService> _logger;
    private readonly TimeSpan _sessionTimeout = TimeSpan.FromHours(24);
    private const string SESSION_PREFIX = "session";

    public SessionService(ICacheService cacheService, 
        ILogger<SessionService> logger)
    {
        _cacheService = cacheService;
        _logger = logger;
    }
    
    public async Task<string> CreateSessionAsync(int userId)
    {
        var sessionId = GenerateSessionId();
        
        _logger.LogInformation($"세션 아이디 생성: 유저 아이디: {userId}, 세션 아이디: {sessionId}");
        
        await _cacheService.SetAsync(
            sessionId,
            userId.ToString(),
            _sessionTimeout,
            SESSION_PREFIX
        );
        
        _logger.LogInformation($"[CreateSessionAsync] 세션 저장 완료: SessionId={sessionId}, TTL={_sessionTimeout}");
        
        return sessionId;
    }
    
    private string GenerateSessionId()
    {
        return Guid.NewGuid().ToString();
    }

    public async Task<bool> ValidateSessionAsync(string sessionId, int? userId = null)
    {
        _logger.LogInformation($"[DEBUG] ValidateSessionAsync - SessionKey={sessionId}");
        
        var exists = await _cacheService.ExistsAsync(sessionId, SESSION_PREFIX);
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

    public async Task<bool> ValidateAndRefreshSessionAsync(string sessionId, int? userId = null)
    {
        if (!await ValidateSessionAsync(sessionId, userId))
            return false;
        
        await _cacheService.SetExpirationAsync(sessionId, _sessionTimeout, SESSION_PREFIX);
        _logger.LogInformation($"세션 TTL 연장: SessionId={sessionId}, TTL={_sessionTimeout}");
    
        return true;
    }

    public async Task<int> GetUserIdBySessionIdAsync(string sessionId)
    {
        var value = await _cacheService.GetAsync(sessionId, SESSION_PREFIX);
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
        
        await _cacheService.RemoveAsync(sessionId, SESSION_PREFIX);
        
        _logger.LogInformation($"[DeleteSessionAsync] 세션 삭제 완료: SessionId={sessionId}");
    }
}