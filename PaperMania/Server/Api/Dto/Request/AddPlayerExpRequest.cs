namespace Server.Api.Dto.Request
{
    /// <summary>
    /// 플레이어 경험치 추가 요청 DTO
    /// </summary>
    public class AddPlayerExpRequest
    {
        /// <summary>
        /// 추가할 새로운 경험치 양
        /// </summary>
        public int NewExp { get; set; }
    }
}