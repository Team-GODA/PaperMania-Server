namespace Server.Domain.Entity;

public class PlayerCharacterData
{
    public int UserId { get; set; }
    public string CharacterId { get; set; } = null!;
        
    public int CharacterExp { get; set; } = 0;
    public int CharacterLevel { get; set; } = 1;
    public int NormalSkillLevel { get; set; } = 1;
    public int EpicSkillLevel { get; set; } = 1;
    public int PieceAmount { get; set; } = 0;
    
    public CharacterData Data { get; set; } = null!;
}