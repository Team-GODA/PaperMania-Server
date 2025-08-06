namespace Server.Api.Dto.Request
{
    /// <summary>
    /// 회원가입 요청 DTO
    /// </summary>
    public class RegisterRequest
    {
        /// <summary>
        /// 플레이어 고유 ID
        /// </summary>
        public string PlayerId { get; set; } = null!;
        
        /// <summary>
        /// 이메일 주소
        /// </summary>
        public string Email { get; set; } = null!;
        
        /// <summary>
        /// 비밀번호
        /// </summary>
        public string Password { get; set; } = null!;
    }
}