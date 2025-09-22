namespace Server.Domain.Entity;

public class CharacterData
{
    public string CharacterId { get; set; } = null!;
    public string CharacterName { get; set; } = null!; 
    public int Rarity { get; set; }
    public int PieceAmount { get; set; } = 0;
    public int Position { get; set; }
}