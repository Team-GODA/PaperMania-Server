namespace Server.Domain.Entity;

using Server.Api.Dto.Response;
using Server.Application.Exceptions;

public class Currency
{
    public int UserId { get; }
    public int ActionPoint { get; set; }
    public int MaxActionPoint { get; set; }
    public int Gold { get; set; }
    public int PaperPiece { get; set; }
    public DateTime LastActionPointUpdated { get; set; }

    public Currency(
        int userId,
        int actionPoint,
        int maxActionPoint,
        int gold,
        int paperPiece,
        DateTime lastActionPointUpdated)
    {
        if (userId <= 0)
            throw new RequestException(
                ErrorStatusCode.BadRequest,
                "INVALID_USER_ID");

        UserId = userId;
        MaxActionPoint = maxActionPoint;
        ActionPoint = actionPoint;
        Gold = gold;
        PaperPiece = paperPiece;
        LastActionPointUpdated = lastActionPointUpdated;
    }

    public void SetMaxActionPoint(int amount)
    {
        if (amount <= 0)
            throw new RequestException(
                ErrorStatusCode.BadRequest,
                "INVALID_MAX_ACTION_POINT");
        
        MaxActionPoint = amount;
    }

    public void SetActionPoint(int amount)
    {
        if (amount < 0)
            throw new RequestException(
                ErrorStatusCode.BadRequest,
                "INVALID_ACTION_POINT");

        if (amount > MaxActionPoint)
            amount = MaxActionPoint;
        
        ActionPoint = amount;
    }

    public void SpendActionPoint(int amount, DateTime nowUtc)
    {
        if (amount <= 0)
            throw new RequestException(
                ErrorStatusCode.BadRequest,
                "INVALID_ACTION_POINT_AMOUNT");

        if (ActionPoint < amount)
            throw new RequestException(
                ErrorStatusCode.BadRequest,
                "INSUFFICIENT_ACTION_POINT");

        ActionPoint = Math.Max(ActionPoint - amount, 0);
        LastActionPointUpdated = nowUtc;
    }

    public void SetActionPointToMax(DateTime nowUtc)
    {
        if (MaxActionPoint <= 0)
            throw new RequestException(
                ErrorStatusCode.BadRequest,
                "INVALID_MAX_ACTION_POINT");

        ActionPoint = MaxActionPoint;
        LastActionPointUpdated = nowUtc;
    }
    
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

