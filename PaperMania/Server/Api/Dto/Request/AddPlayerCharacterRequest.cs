using Server.Domain.Entity;
using Server.Infrastructure.StaticData;

namespace Server.Api.Dto.Request
{
    /// <summary>
    /// 플레이어가 보유한 캐릭터 추가 요청 DTO
    /// </summary>
    public class AddPlayerCharacterRequest
    {
        public int Id { get; set; }
        public CharacterData Data { get; set; } = null!;
    }
}