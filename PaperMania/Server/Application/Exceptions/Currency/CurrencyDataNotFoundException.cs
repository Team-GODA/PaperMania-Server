using Server.Api.Dto.Response;

namespace Server.Application.Exceptions.Currency;

public class CurrencyDataNotFoundException : GameException
{
    public CurrencyDataNotFoundException(int? userId)
        : base(ErrorStatusCode.NotFound, $"Id : {userId}의 재화 데이터가 없습니다.")
    {
    }
}