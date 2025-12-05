namespace Server.Infrastructure.Cache;

public static class CacheKey
{

    // Player 관련 클래스
    public static class Player
    {
        /// <summary>
        /// 계정 데이터 (UserId로 조회)
        /// </summary>
        public static string Account(int userId)
            => $"Player:Account:{userId}";
        
        /// <summary>
        /// 계정 데이터 (PlayerId로 조회)
        /// </summary>
        public static string AccountByPlayerId(string playerId)
            => $"Player:Account:PlayerId:{playerId}";
        
        /// <summary>
        /// 게임 데이터
        /// </summary>
        public static string GameData(int userId)
            => $"Player:Game:{userId}";
        
        /// <summary>
        /// 재화 데이터
        /// </summary>
        public static string CurrencyData(int userId)
            => $"Player:Currency:{userId}";
    }

    public static class Session
    {
        /// <summary>
        /// 세션 데이터 (Primary: SessionId로 UserId 찾기)
        /// </summary>
        public static string ById(string sessionId)
            => $"Session:{sessionId}"
        
        /// <summary>
        /// 세션 역방향 매핑 (Secondary: UserId로 SessionId 찾기)
        /// </summary>
        public static string ByUserId(int userId)
            => $"Session:UserId:{userId}";
    }
}