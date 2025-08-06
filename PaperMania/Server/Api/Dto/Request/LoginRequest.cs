/// <summary>
/// 로그인 요청 데이터
/// </summary>
public class LoginRequest
{
    /// <summary>게임 내 고유 사용자 ID</summary>
    public string PlayerId { get; set; }

    /// <summary>비밀번호</summary>
    public string Password { get; set; }
}