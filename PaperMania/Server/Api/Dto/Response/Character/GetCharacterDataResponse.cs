using Server.Infrastructure.Persistence.Model;

namespace Server.Api.Dto.Response.Character;

public class GetCharacterDataResponse
{
    public PlayerCharacterData Character { get; set; }
}