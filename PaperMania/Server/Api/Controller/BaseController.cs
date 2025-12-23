using Microsoft.AspNetCore.Mvc;
using Server.Api.Dto.Response;
using Server.Application.Exceptions;

namespace Server.Api.Controller;

public abstract class BaseController : ControllerBase
{
    protected int GetUserId()
    {
        if (!HttpContext.Items.TryGetValue("UserId", out var userIdObj)
            || userIdObj is not int userId)
        {
            throw new RequestException(
                ErrorStatusCode.Unauthorized,
                "INVALID_SESSION");
        }

        return userId;
    }

    protected string GetSessionId()
    {
        if (!HttpContext.Items.TryGetValue("SessionId", out var sessionIdObj)
            || sessionIdObj is not string sessionId)
        {
            throw new RequestException(
                ErrorStatusCode.Unauthorized,
                "INVALID_SESSION");
        }
            
        return sessionId;
    }
}