namespace Server.Domain.Entity;

public class CharacterData
{
    public string CharacterId { get; set; } = null!;
    public string CharacterName { get; set; } = null!; 
    public string RarityString
    {
        get => Rarity.ToString();
        set => Rarity = Enum.Parse<Rarity>(value, ignoreCase: true);
    }
    
    [System.Text.Json.Serialization.JsonIgnore]
    public Rarity Rarity { get; set; }
}

public enum Rarity
{
    Common,
    Rare,
    Epic
}