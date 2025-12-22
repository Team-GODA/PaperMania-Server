using Server.Infrastructure.StaticData.Model;

namespace Server.Application.Port.Output.StaticData;

public interface ILevelDefinitionStore
{
    LevelDefinition? GetLevelDefinition(int level);
}