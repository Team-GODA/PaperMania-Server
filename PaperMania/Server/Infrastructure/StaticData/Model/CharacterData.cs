namespace Server.Infrastructure.StaticData.Model;

public record CharacterData(
    int CharacterId,
    string CharacterName,
    CharacterRole Role,
    CharacterRarity Rarity,
    float BaseHP,
    float BaseATK,
    int NormalSkillId,
    int UltimateSkillId,
    int SupportSkillId
);

public enum CharacterRole
{
    Main = 1,
    Support = 2
}

public enum CharacterRarity
{
    Common = 1,
    Rare = 2,
    Epic = 3
}