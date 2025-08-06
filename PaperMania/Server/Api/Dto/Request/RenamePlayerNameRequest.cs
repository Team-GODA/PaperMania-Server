namespace Server.Api.Dto.Request
{
    /// <summary>
    /// 플레이어 이름 변경 요청 DTO
    /// </summary>
    public class RenamePlayerNameRequest
    {
        /// <summary>
        /// 변경할 새 플레이어 이름
        /// </summary>
        public string? NewName { get; set; }
    }
}