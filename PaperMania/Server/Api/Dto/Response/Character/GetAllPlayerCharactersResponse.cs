using Server.Domain.Entity;
using Server.Infrastructure.Persistence.Model;

namespace Server.Api.Dto.Response.Character;

public class GetAllPlayerCharactersResponse
{
    public IEnumerable<PlayerCharacterData> PlayerCharacters { get; set; }
}