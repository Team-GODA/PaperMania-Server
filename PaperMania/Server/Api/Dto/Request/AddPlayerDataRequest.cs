namespace Server.Api.Dto.Request
{
    /// <summary>
    /// 플레이어 데이터 추가 요청 DTO
    /// </summary>
    public class AddPlayerDataRequest
    {
        /// <summary>
        /// 플레이어 이름
        /// </summary>
        public string PlayerName { get; set; } = null!;
    }
}