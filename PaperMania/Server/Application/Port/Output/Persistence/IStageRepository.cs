using Server.Infrastructure.Persistence.Model;

namespace Server.Application.Port.Output.Persistence;

public interface IStageRepository
{
    Task<PlayerStageData?> FindByUserIdAsync(int userId, int stageNum, int stageSubNum);
    Task CreateAsync(PlayerStageData data);
}