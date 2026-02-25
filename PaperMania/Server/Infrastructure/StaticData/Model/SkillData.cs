namespace Server.Infrastructure.StaticData.Model;

public record SkillData(
    int SkillId,
    string SkillName,
    SkillType SkillType,
    float CoolTime,
    SkillScalingType ScalingType,
    SkillTargetType TargetType
);

public enum SkillType
{
    Normal = 1,
    Ultimate = 2,
    Support = 3
}

public enum SkillScalingType
{
    ATK = 1,
    HP = 2
}

public enum SkillTargetType
{
    Enemy = 1,
    EnemyAll = 2,
    Ally = 3,
    Self = 4
}