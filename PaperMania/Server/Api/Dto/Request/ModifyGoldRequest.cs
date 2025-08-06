namespace Server.Api.Dto.Request;

/// <summary>
/// 플레이어 골드 수량 수정 요청 DTO
/// </summary>
public class ModifyGoldRequest
{
    /// <summary>
    /// 변경할 골드 양 (양수: 획득, 음수: 사용)
    /// </summary>
    public int Amount { get; set; }
}