using TaskListApi.Extensions;
using TaskListApi.Services;
using TaskListApi.Services.Impl;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSwaggerWithAuthorize();
builder.Services.AddMongo(builder.Configuration);

builder.Services.AddScoped<ITaskListService, TaskListService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandling();
app.UseCurrentUser();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
