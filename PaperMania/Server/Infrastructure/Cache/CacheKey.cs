namespace Server.Infrastructure.Cache;

public static class CacheKey
{
    /// <summary>
    /// 플레이어 계정 데이터
    /// Key: PlayerAccountData:{userId}
    /// </summary>
    public static string PlayerAccountDataByUserId(int userId)
        => $"PlayerAccountData:{userId}";
    
    /// <summary>
    /// 플레이어 계정 데이터
    /// Key: PlayerAccountData:PlayerId:{playerId}
    /// </summary>
    public static string PlayerAccountDataByPlayerId(string playerId)
        => $"PlayerAccountData:PlayerId:{playerId}";
    
    /// <summary>
    /// 플레이어 게임 데이터
    /// Key: PlayerGameData:{userId}
    /// </summary>
    public static string PlayerGameData(int userId)
        => $"PlayerGameData:{userId}";
    
    /// <summary>
    /// 세션 데이터
    /// Key: Session:{sessionId}
    /// </summary>
    public static string Session(string sessionId)
        => $"Session:{sessionId}";
    
    /// <summary>
    /// UserId로 SessionId 조회
    /// Key: Session:UserId:{userId}
    /// </summary>
    public static string SessionByUserId(int? userId)
        => $"Session:UserId:{userId}";
}