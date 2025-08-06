namespace Server.Api.Dto.Request
{
    /// <summary>
    /// 구글 로그인 요청 DTO
    /// </summary>
    public class GoogleLoginRequest
    {
        /// <summary>
        /// 구글 인증에서 받은 ID 토큰
        /// </summary>
        public string IdToken { get; set; } = null!;
    }
}