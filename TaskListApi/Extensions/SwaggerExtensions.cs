using Microsoft.OpenApi.Models;

namespace TaskListApi.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerWithAuthorize(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title       = "TaskList API",
                Version     = "v1",
                Description = "REST backend for sharing task lists"
            });
            
            c.AddSecurityDefinition("UserIdHeader", new OpenApiSecurityScheme
            {
                Name = "X-User-Id",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Description = "User identifier (GUID)"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "UserIdHeader"
                        }
                    },
                    []
                }
            });
        });

        return services;
    }
}
