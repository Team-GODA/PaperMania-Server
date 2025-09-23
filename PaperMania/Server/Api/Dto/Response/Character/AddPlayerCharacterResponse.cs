using Server.Domain.Entity;

namespace Server.Api.Dto.Response.Character;

public class AddPlayerCharacterResponse
{
    public int Id { get; set; }
    public CharacterData Data { get; set; } = null!;
}