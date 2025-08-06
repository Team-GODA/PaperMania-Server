namespace Server.Api.Dto.Request
{
    /// <summary>
    /// 플레이어가 보유한 캐릭터 추가 요청 DTO
    /// </summary>
    public class AddPlayerCharacterRequest
    {
        /// <summary>
        /// 플레이어 고유 ID
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// 추가할 캐릭터의 ID
        /// </summary>
        public string CharacterId { get; set; } = null!;
    }
}