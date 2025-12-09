using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Server.Api.Dto.Response;
using Server.Application.Port;

namespace Server.Api.Filter;

public class SessionValidationFilter : IAsyncActionFilter
{
    private readonly ILogger<SessionValidationFilter> _logger;
    private readonly ISessionService _sessionService;

    public SessionValidationFilter(ILogger<SessionValidationFilter> logger,  ISessionService sessionService)
    {
        _logger = logger;
        _sessionService = sessionService;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue("Session-Id", out var sessionId)
            || string.IsNullOrWhiteSpace(sessionId))
        {
            _logger.LogWarning("세션 ID가 없습니다.");
            context.Result = new JsonResult(
                ApiResponse.Error<EmptyResponse>(ErrorStatusCode.Unauthorized, 
                    "SESSION_ID_REQUIRED")
                );
            
            return;
        }
        
        var userId = await _sessionService.FindUserIdBySessionIdAsync(sessionId!);

        var isValid = await _sessionService.ValidateSessionAsync(sessionId!); 
        if (!isValid)
        {
            _logger.LogWarning("유효하지 않은 세션");
            context.Result = new JsonResult(ApiResponse.Error<EmptyResponse>(ErrorStatusCode.Unauthorized,
                "INVALID_SESSION"));
            return;
        }

        context.HttpContext.Items["SessionId"] = sessionId.ToString();
        context.HttpContext.Items["UserId"] = userId;

        await next();
    }
}