namespace TaskListApi.Middlewares;

public class CurrentUserMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue("X-User-Id", out var headerValue) ||
            !Guid.TryParse(headerValue, out var userId) ||
            userId == Guid.Empty)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync("Missing or invalid X-User-Id");
        }
        else
        {
            context.Items["UserId"] = userId;
            await next(context);
        }
    }
}
