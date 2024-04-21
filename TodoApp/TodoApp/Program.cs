using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using TodoApp.Services;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

var services = builder.Services;

services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });;
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddDbContext<TodoDbContext>(x => x.UseSqlite(config.GetConnectionString("DefaultConnection")));

services.AddScoped<ITodoItemService, TodoItemService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

if(config.GetValue("DO_MIGRATE", false) == true)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
    db.Database.Migrate();
}

app.Run();