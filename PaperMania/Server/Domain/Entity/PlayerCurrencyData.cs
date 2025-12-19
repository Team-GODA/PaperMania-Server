using Server.Api.Dto.Response;
using Server.Application.Exceptions;

namespace Server.Domain.Entity;

public class PlayerCurrencyData
{
    public int UserId { get; set; }
    public int ActionPoint { get; set; }
    public int MaxActionPoint { get; set; }
    public int Gold { get; set; }
    public int PaperPiece { get; set; }
    public DateTime LastActionPointUpdated { get; set; }

    public void SpendGold(int amount)
    {
        if (amount <= 0)
            throw new RequestException(
                ErrorStatusCode.BadRequest,
                "INVALID_GOLD_AMOUNT");
        
        if (Gold < amount)
            throw new RequestException(
                ErrorStatusCode.Conflict,
                "NOT_ENOUGH_GOLD");
        
        Gold = Math.Max(Gold - amount, 0);
    }
    
    public void GainGold(int amount)
    {
        if (amount <= 0)
            throw new RequestException(
                ErrorStatusCode.BadRequest,
                "INVALID_GOLD_AMOUNT");

        Gold += amount;
    }

    public void SpendPaperPiece(int amount)
    {
        if (amount <= 0)
            throw new RequestException(
                ErrorStatusCode.BadRequest,
                "INVALID_PAPER_PIECE_AMOUNT");
        
        if (PaperPiece < amount)
            throw new RequestException(
                ErrorStatusCode.Conflict,
                "NOT_ENOUGH_PAPER_PIECE");
        
        PaperPiece = Math.Max(PaperPiece - amount, 0);
    }

    public void GainPaperPiece(int amount)
    {
        if (amount <= 0)
            throw new RequestException(
                ErrorStatusCode.BadRequest,
                "INVALID_PAPER_PIECE_AMOUNT");
        
        PaperPiece += amount;
    }
}