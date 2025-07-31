using System.Net;
using Server.Api.Dto.Response;
using Server.Application.Exceptions;

namespace Server.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (GameException ex)
        {
            _logger.LogWarning(ex, "GameException 발생");
            await WriteErrorAsync(context, ex.StatusCode, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled 예외 발생");
            await WriteErrorAsync(context, ErrorStatusCode.ServerError, "알 수 없는 오류가 발생했습니다.");
        }
    }

    private async Task WriteErrorAsync(HttpContext context, ErrorStatusCode errorCode, string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.OK;

        var response = ApiResponse.Error<object>(errorCode, message);
        await context.Response.WriteAsJsonAsync(response);
    }
}