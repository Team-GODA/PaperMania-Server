namespace Server.Api.Dto.Request
{
    /// <summary>
    /// 스테이지 보상 수령 요청 DTO
    /// </summary>
    public class ClaimStageRewardRequest
    {
        /// <summary>
        /// 스테이지 번호
        /// </summary>
        public int StageNum { get; set; }

        /// <summary>
        /// 스테이지 서브 번호
        /// </summary>
        public int SubStageNum { get; set; }
    }
}