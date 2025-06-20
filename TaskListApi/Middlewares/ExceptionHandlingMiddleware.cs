using System.Net;
using System.Text.Json;
using TaskListApi.Exceptions;

namespace TaskListApi.Middlewares;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ForbiddenOrNotFound ex)
        {
            logger.LogWarning(ex, "Forbidden or not found error occurred.");
            await WriteError(context, HttpStatusCode.NotFound, "Not found.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception occurred.");
            await WriteError(context, HttpStatusCode.InternalServerError,
                "An unexpected error occurred.", ex.Message);
        }
    }
    
    private static async Task WriteError(HttpContext ctx, HttpStatusCode code,
        string message, string? details = null)
    {
        ctx.Response.StatusCode  = (int)code;
        ctx.Response.ContentType = "application/json";

        object errorResponse = details is null
            ? new { message }
            : new { message, details };
        
        var json = JsonSerializer.Serialize(errorResponse);
        await ctx.Response.WriteAsync(json);
    }
}