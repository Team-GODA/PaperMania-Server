using Server.Api.Dto.Response;

namespace Server.Application.Exceptions.Currency;

public class NotEnoughPaperPieceException : GameException
{
    public NotEnoughPaperPieceException(int? userId)
        : base(ErrorStatusCode.Conflict, $"Id : {userId}의 종이 조각이 부족합니다.")
    {
    }
}