namespace Server.Domain.Entity;

public class PlayerCharacterData
{
    public int UserId { get; set; }

    public int CharacterExp { get; set; }
    public int CharacterLevel { get; set; } = 1;
    public int NormalSkillLevel { get; set; } = 1;
    public int EpicSkillLevel { get; set; } = 1;
    public int PieceAmount { get; set; }
    
    public CharacterData? Data { get; set; }
}