using Server.Infrastructure.Persistence.Model;

namespace Server.Api.Dto.Response.Character;

public class GetAllPlayerCharactersResponse
{
    public List<PlayerCharacterData> Characters { get; set; }
}