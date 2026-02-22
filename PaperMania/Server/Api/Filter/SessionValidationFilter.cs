using Microsoft.AspNetCore.Mvc.Filters;
using Server.Api.Dto.Response;
using Server.Application.Exceptions;
using Server.Application.Port.Output.Service;

namespace Server.Api.Filter;

public class SessionValidationFilter : IAsyncActionFilter
{
    private readonly ILogger<SessionValidationFilter> _logger;
    private readonly ISessionService _sessionService;

    public SessionValidationFilter(
        ILogger<SessionValidationFilter> logger,
        ISessionService sessionService)
    {
        _logger = logger;
        _sessionService = sessionService;
    }

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        if (HttpMethods.IsOptions(context.HttpContext.Request.Method))
        {
            await next();
            return;
        }

        if (!context.HttpContext.Request.Headers.TryGetValue("Session-Id", out var sessionId)
            || string.IsNullOrWhiteSpace(sessionId))
        {
            _logger.LogWarning("세션 ID가 없습니다.");
            throw new RequestException(
                ErrorStatusCode.Unauthorized,
                "SESSION_ID_REQUIRED"
            );
        }

        var ct = context.HttpContext.RequestAborted;
        var isValid = await _sessionService.ValidateSessionAsync(sessionId!, ct);
        if (!isValid)
        {
            _logger.LogWarning("유효하지 않은 세션");
            throw new RequestException(
                ErrorStatusCode.Unauthorized,
                "INVALID_SESSION"
            );
        }

        var userId = await _sessionService.FindUserIdBySessionIdAsync(sessionId!, ct);

        context.HttpContext.Items["SessionId"] = sessionId.ToString();
        context.HttpContext.Items["UserId"] = userId;

        await next();
    }
}