using TaskListApi.Services;
using TaskListApi.Services.Impl;

namespace TaskListApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddOpenApi();

        builder.Services.AddMongo(builder.Configuration);
        
        builder.Services.AddScoped<ITaskListService, TaskListService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseExceptionHandling();
        
        app.UseHttpsRedirection();

        app.MapControllers();

        app.Run();
    }
}