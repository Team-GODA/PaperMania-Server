using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Server.Api.Dto.Response;
using Server.Application.Port;

namespace Server.Api.Filter;

public class SessionValidationFilter : IAsyncActionFilter
{
    private readonly ILogger<SessionValidationFilter> _logger;
    private readonly ICacheService _cacheService;

    public SessionValidationFilter(ILogger<SessionValidationFilter> logger, ICacheService cacheService)
    {
        _logger = logger;
        _cacheService = cacheService;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue("Session-Id", out var sessionId) || string.IsNullOrWhiteSpace(sessionId))
        {
            _logger.LogWarning("세션 ID가 없습니다.");
            context.Result = new JsonResult(ApiResponse.Error<EmptyResponse>(ErrorStatusCode.Unauthorized, "SID가 없습니다."));
            return;
        }

        var isValid = await _cacheService.ExistsAsync(sessionId!);
        if (!isValid)
        {
            _logger.LogWarning($"유효하지 않은 세션: {sessionId}");
            context.Result = new JsonResult(ApiResponse.Error<EmptyResponse>(ErrorStatusCode.Unauthorized, "유효하지 않은 세션입니다."));
            return;
        }

        context.HttpContext.Items["SessionId"] = sessionId.ToString();

        await next();
    }
}