using Server.Domain.Entity;
using Server.Infrastructure.Persistence.Model;

namespace Server.Domain.Service;

public class ActionPointService
{
    private const int RegenerationIntervalMinutes = 4;
    private const int RegenerationAmount = 1;

    public bool TryRegenerate(CurrencyData data, DateTime nowUtc)
    {
        if (data.ActionPoint >= data.MaxActionPoint)
            return false;

        var elapsed = nowUtc - data.LastActionPointUpdated;
        var intervalsElapsed =
            (int)(elapsed.TotalMinutes / RegenerationIntervalMinutes);

        if (intervalsElapsed <= 0)
            return false;

        var pointsToAdd = intervalsElapsed * RegenerationAmount;
        var newActionPoint = Math.Min(
            data.MaxActionPoint,
            data.ActionPoint + pointsToAdd
        );

        var actualAdded = newActionPoint - data.ActionPoint;
        var actualIntervalsUsed = actualAdded / RegenerationAmount;

        data.ActionPoint = newActionPoint;
        data.LastActionPointUpdated =
            data.LastActionPointUpdated.AddMinutes(
                actualIntervalsUsed * RegenerationIntervalMinutes);

        return true;
    }
}