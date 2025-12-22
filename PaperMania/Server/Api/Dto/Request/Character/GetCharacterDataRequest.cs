using Server.Infrastructure.Persistence.Model;

namespace Server.Api.Dto.Request.Character;

public class GetCharacterDataRequest
{
    public PlayerCharacterData Character { get; set; }
}