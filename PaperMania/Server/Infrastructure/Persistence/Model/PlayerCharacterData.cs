namespace Server.Infrastructure.Persistence.Model;

public class PlayerCharacterData
{
    public int UserId { get; set; }
    public int CharacterId { get; set; }
    public int CharacterLevel { get; set; } = 1;
    public int CharacterExp { get; set; } = 0;
    public int NormalSkillLevel { get; set; } 
    public int UltimateSkillLevel { get; set; }
    public int SupportSkillLevel { get; set; }
    public int PieceAmount { get; set; }
    
    public PlayerCharacterData() { }

    public PlayerCharacterData(int userId, int characterId)
    {
        UserId = userId;
        CharacterId = characterId;
    }
}