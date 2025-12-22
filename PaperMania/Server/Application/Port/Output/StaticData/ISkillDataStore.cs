using Server.Infrastructure.StaticData.Model;

namespace Server.Application.Port.Output.StaticData;

public interface ISkillDataStore
{
    SkillData? Get(int skillId);
    bool Contains(int skillId);
}