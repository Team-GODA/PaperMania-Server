using Server.Api.Dto.Response;

namespace Server.Application.Exceptions.Currency;

public class NotEnoughGoldException : GameException
{
    public NotEnoughGoldException(int? userId)
        : base(ErrorStatusCode.Conflict, $"Id : {userId}의 골드가 부족합니다.")
    {
    }
}