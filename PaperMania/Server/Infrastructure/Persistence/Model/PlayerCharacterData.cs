namespace Server.Infrastructure.Persistence.Model;

public record PlayerCharacterData(
    int UserId,
    int CharacterId,
    int CharacterLevel = 1,
    int CharacterExp = 0,
    int NormalSkillLevel = 1,
    int UltimateSkillLevel = 1,
    int SupportSkillLevel = 1
);