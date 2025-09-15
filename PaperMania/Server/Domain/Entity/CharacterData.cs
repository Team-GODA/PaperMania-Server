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
    public string PositionString
    {
        get => Position.ToString();
        set => Position = Enum.Parse<Position>(value, ignoreCase: true);
    }
    public int PieceAmount { get; set; } = 0;
    
    [System.Text.Json.Serialization.JsonIgnore]
    public Rarity Rarity { get; set; }
    [System.Text.Json.Serialization.JsonIgnore]
    public Position Position { get; set; }
}

public enum Rarity
{
    Common,
    Rare,
    Epic
}

public enum Position
{
    Front,
    Middle,
    Back
}