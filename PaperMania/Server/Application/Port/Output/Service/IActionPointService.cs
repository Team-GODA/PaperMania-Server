using Server.Domain.Entity;

namespace Server.Application.Port.Output.Service;

public interface IActionPointService
{
    bool TryRegenerate(CurrencyData data, DateTime now);
}