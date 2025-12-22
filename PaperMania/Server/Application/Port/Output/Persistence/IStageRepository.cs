using Server.Domain.Entity;
using Server.Infrastructure.Persistence.Model;

namespace Server.Application.Port.Output.Persistence;

public interface IStageRepository
{
    Task CreatePlayerStageDataAsync(int? userId);
    Task<bool> IsClearedStageAsync(PlayerStageData data);
    Task UpdateIsClearedAsync(PlayerStageData data);
}