namespace Server.Api.Dto.Request
{
    /// <summary>
    /// 플레이어 행동력 사용 요청 DTO
    /// </summary>
    public class UsePlayerActionPointRequest
    {
        /// <summary>
        /// 사용할 행동력 양
        /// </summary>
        public int UsedActionPoint { get; set; }
    }
}