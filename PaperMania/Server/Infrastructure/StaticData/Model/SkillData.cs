namespace Server.Infrastructure.StaticData.Model;

public class SkillData
{
    public int SkillId { get; set; }
    public string SkillName { get; set; }

    public SkillType SkillType { get; set; }
    public float CoolTime { get; set; }

    public float Value { get; set; }
    public SkillScalingType ScalingType { get; set; }
    public SkillTargetType TargetType { get; set; }
}

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