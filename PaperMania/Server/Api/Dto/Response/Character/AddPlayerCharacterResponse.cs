using Server.Domain.Entity;
using Server.Infrastructure.StaticData;

namespace Server.Api.Dto.Response.Character;

public class AddPlayerCharacterResponse
{
    public int Id { get; set; }
    public CharacterData Data { get; set; } = null!;
}