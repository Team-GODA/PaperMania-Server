using Server.Infrastructure.StaticData.Model;

namespace Server.Infrastructure.Persistence.Model;

public class PlayerCharacterData
{
    public int UserId { get; set; }

    public int CharacterId { get; set; }
    public int CharacterLevel { get; set; } = 1;
    public int CharacterExp { get; set; }
    public int NormalSkillLevel { get; set; } = 1;
    public int UltimateSkillLevel { get; set; } = 1;
    public int SupportSkillLevel { get; set; } = 1;
}