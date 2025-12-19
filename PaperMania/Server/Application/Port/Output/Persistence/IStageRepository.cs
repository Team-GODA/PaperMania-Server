using Server.Domain.Entity;

namespace Server.Application.Port.Output.Persistence;

public interface IStageRepository
{
    Task CreatePlayerStageDataAsync(int? userId);
    Task<bool> IsClearedStageAsync(PlayerStageData data);
    Task UpdateIsClearedAsync(PlayerStageData data);
}