using Microsoft.AspNetCore.Mvc.Filters;
using Server.Api.Attribute;
using System.Diagnostics;

namespace Server.Api.Filter;

public class ApiLogActionFilter : IAsyncActionFilter
{
    private readonly ILogger<ApiLogActionFilter> _logger;

    public ApiLogActionFilter(ILogger<ApiLogActionFilter> logger)
    {
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var stopwatch = Stopwatch.StartNew();

        var apiLog = context.ActionDescriptor.EndpointMetadata
            .OfType<ApiLogAttribute>()
            .FirstOrDefault();

        if (apiLog == null)
        {
            await next();
            return;
        }

        var httpMethod = context.HttpContext.Request.Method;
        var path = context.HttpContext.Request.Path;

        var sessionId = context.HttpContext.Request.Headers["Session-Id"].FirstOrDefault();
        var userId = context.HttpContext.Items.TryGetValue("UserId", out var uid)
            ? uid
            : null;

        _logger.LogInformation(
            "[{Domain}][{Method}] Request - Path: {Path}, UserId: {UserId}, SessionId: {SessionId}",
            apiLog.Domain,
            httpMethod,
            path,
            userId,
            sessionId
        );

        ActionExecutedContext? executedContext;

        try
        {
            executedContext = await next();
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            _logger.LogError(
                ex,
                "[{Domain}][{Method}] Exception - Path: {Path}, ElapsedMs: {ElapsedMs}",
                apiLog.Domain,
                httpMethod,
                path,
                stopwatch.ElapsedMilliseconds
            );

            throw;
        }

        stopwatch.Stop();

        if (executedContext.Exception == null)
        {
            _logger.LogInformation(
                "[{Domain}][{Method}] Success - Path: {Path}, ElapsedMs: {ElapsedMs}",
                apiLog.Domain,
                httpMethod,
                path,
                stopwatch.ElapsedMilliseconds
            );
        }
    }
}
