using Server.Domain.Entity;
using Server.Infrastructure.Persistence.Model;

namespace Server.Api.Dto.Response.Character;

public class GetCharacterDataResponse
{
    public PlayerCharacter Character { get; set; }
}