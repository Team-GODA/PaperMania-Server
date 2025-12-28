using Server.Api.Dto.Response;
using Server.Application.Exceptions;

namespace Server.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (HttpMethods.IsOptions(context.Request.Method))
        {
            context.Response.StatusCode = StatusCodes.Status204NoContent;
            return;
        }

        try
        {
            await _next(context);
        }
        catch (GameException ex)
        {
            _logger.LogWarning(ex, "GameException 발생");
            await WriteGameErrorAsync(context, ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled 예외 발생");
            await WriteServerErrorAsync(context);
        }
    }

    private static async Task WriteGameErrorAsync(HttpContext context, GameException ex)
    {
        context.Response.ContentType = "application/json";

        context.Response.StatusCode = MapToHttpStatus(ex.StatusCode);

        var response = ApiResponse.Error<object>(ex.StatusCode, ex.Message);
        await context.Response.WriteAsJsonAsync(response);
    }

    private static async Task WriteServerErrorAsync(HttpContext context)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var response = ApiResponse.Error<object>(
            ErrorStatusCode.ServerError,
            "알 수 없는 오류가 발생했습니다."
        );

        await context.Response.WriteAsJsonAsync(response);
    }

    private static int MapToHttpStatus(ErrorStatusCode code)
    {
        return code switch
        {
            ErrorStatusCode.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorStatusCode.BadRequest   => StatusCodes.Status400BadRequest,
            ErrorStatusCode.NotFound     => StatusCodes.Status404NotFound,
            ErrorStatusCode.Conflict     => StatusCodes.Status409Conflict,
            ErrorStatusCode.ServerError  => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status400BadRequest
        };
    }
}
