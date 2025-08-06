namespace Server.Api.Dto.Request
{
    /// <summary>
    /// 플레이어 최대 행동력 갱신 요청 DTO
    /// </summary>
    public class UpdatePlayerMaxActionPointRequest
    {
        /// <summary>
        /// 새로 설정할 최대 행동력 값
        /// </summary>
        public int NewMaxActionPoint { get; set; }
    }
}