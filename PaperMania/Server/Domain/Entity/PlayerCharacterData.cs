namespace Server.Domain.Entity;

public class PlayerCharacterData
{
    public int Id { get; set; }
    public string CharacterId { get; set; } = null!;
    public string CharacterName { get; set; } = null!;
    public int CharacterLevel { get; set; } = 1;
    public int NormalSkillLevel { get; set; } = 1;
    public int EpicSkillLevel { get; set; } = 1;
    public string RarityString
    {
        get => Rarity.ToString();
        init => Rarity = Enum.Parse<Rarity>(value, ignoreCase: true);
    }
    
    [System.Text.Json.Serialization.JsonIgnore]
    public Rarity Rarity { get; set; }
}