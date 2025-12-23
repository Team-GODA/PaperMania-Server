namespace Server.Api.Dto.Request.Character
{
    /// <summary>
    /// 플레이어가 보유한 캐릭터 추가 요청 DTO
    /// </summary>
    public class AddPlayerCharacterRequest
    {
        public int CharacterId { get; set; }
    }
}