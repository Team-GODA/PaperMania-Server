namespace Server.Infrastructure.StaticData.Model;

public class CharacterData
{
    public int CharacterId  { get; set; }
    public string CharacterName { get; set; }
    
    public CharacterRole Role { get; set; }
    public CharacterRarity Rarity { get; set; }
    
    public float BaseHP { get; set; }
    public float BaseATK { get; set; }
    
    public int NormalSkillId  { get; set; }
    public int UltimateSkillId { get; set; }
    public int SupportSkillId { get; set; }
}

public enum CharacterRole
{
    None = 0,
    Main = 1,
    Support = 2
}

public enum CharacterRarity
{
    Common = 1,
    Rare = 2,
    Epic = 3
}
