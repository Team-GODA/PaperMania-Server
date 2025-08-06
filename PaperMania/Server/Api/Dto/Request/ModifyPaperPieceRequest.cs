namespace Server.Api.Dto.Request
{
    /// <summary>
    /// 플레이어 종이조각 수량 수정 요청 DTO
    /// </summary>
    public class ModifyPaperPieceRequest
    {
        /// <summary>
        /// 수정할 종이조각 수량 (양수는 추가, 음수는 사용)
        /// </summary>
        public int Amount { get; set; }
    }
}