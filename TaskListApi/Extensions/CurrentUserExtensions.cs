using TaskListApi.Middlewares;

namespace TaskListApi.Extensions;

public static class CurrentUserExtensions
{
    public static IApplicationBuilder UseCurrentUser(this IApplicationBuilder app)
        => app.UseMiddleware<CurrentUserMiddleware>();
}
