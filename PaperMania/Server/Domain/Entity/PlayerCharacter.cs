using Server.Infrastructure.StaticData.Model;

namespace Server.Domain.Entity;

public class PlayerCharacter
{
    public int UserId { get; set; }
    public int CharacterId { get; set; }
    public int CharacterLevel { get; set; }
    public int CharacterExp { get; set; }
    public CharacterRole Role { get; set; }
    public int NormalSkillLevel { get; set; }
    public int UltimateSkillLevel { get; set; }
    public int SupportSkillLevel { get; set; }
    public int PieceAmount { get; set; }

    public PlayerCharacter(int userId, int characterId, int characterLevel, int characterExp,
        int normalSkillLevel, int ultimateSkillLevel, int supportSkillLevel, int pieceAmount)
    {
        UserId = userId;
        CharacterId = characterId;
        CharacterLevel = characterLevel;
        CharacterExp = characterExp;
        NormalSkillLevel = normalSkillLevel;
        UltimateSkillLevel = ultimateSkillLevel;
        SupportSkillLevel = supportSkillLevel;
        PieceAmount = pieceAmount;
    }

    public static PlayerCharacter Create(int userId, int characterId, CharacterRole role)
    {
        return role switch
        {
            CharacterRole.Main => CreateMain(userId, characterId),
            CharacterRole.Support => CreateSupport(userId, characterId),
            _ => throw new ArgumentException("Invalid character role")
        };
    }
    
    private static PlayerCharacter CreateMain(int userId, int characterId)
    {
        return new PlayerCharacter(userId, characterId,
            characterLevel: 1, characterExp: 0,
            normalSkillLevel: 1, ultimateSkillLevel: 1, supportSkillLevel: 0,
            pieceAmount: 0);
    }

    private static PlayerCharacter CreateSupport(int userId, int characterId)
    {
        return new PlayerCharacter(userId, characterId,
            characterLevel: 1, characterExp: 0,
            normalSkillLevel: 1, ultimateSkillLevel: 0, supportSkillLevel: 1,
            pieceAmount: 0);
    }

    public void AddPiece(int amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Piece amount must be positive");

        PieceAmount += amount;
    }

    public void UpgradeNormalSkill(int maxLevel)
    {
        if (NormalSkillLevel >= maxLevel)
            throw new InvalidOperationException("Normal skill is already at max level");

        NormalSkillLevel++;
    }

    public void UpgradeUltimateSkill(int maxLevel)
    {
        if (UltimateSkillLevel >= maxLevel)
            throw new InvalidOperationException("Ultimate skill is already at max level");

        UltimateSkillLevel++;
    }

    public void UpgradeSupportSkill(int maxLevel)
    {
        if (SupportSkillLevel >= maxLevel)
            throw new InvalidOperationException("Support skill is already at max level");

        SupportSkillLevel++;
    }

    public void GainExp(int amount, Action<int> onLevelUp)
    {
        if (amount <= 0)
            throw new ArgumentException("Exp amount must be positive");

        CharacterExp += amount;
        onLevelUp?.Invoke(CharacterLevel);
    }
}