using Server.Infrastructure.StaticData.Model;

namespace Server.Api.Dto.Response.Character;

public class GetAllCharacterResponse
{
    public List<CharacterData>  Characters { get; set; }
}