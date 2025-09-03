namespace Server.Domain.Entity;

public class PlayerCharacterData
{
    public int UserId { get; set; }
    public string CharacterId { get; set; } = null!;
    public string CharacterName { get; set; } = null!;
    public int CharacterLevel { get; set; } = 1;
    public int NormalSkillLevel { get; set; } = 1;
    public int EpicSkillLevel { get; set; } = 1;
    public string RarityString
    {
        get => Rarity.ToString();
        set => Rarity = Enum.Parse<Rarity>(value, ignoreCase: true);
    }
    public int PieceAmount { get; set; } = 0;
    
    [System.Text.Json.Serialization.JsonIgnore]
    public Rarity Rarity { get; set; }
}