using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:8080")  // Frontend origin
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var connStr = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connStr));

var app = builder.Build();


// Automatically apply migrations, Useful for set-up fails early
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate(); // creates tables if they don't exist
}



var logger = app.Logger;

app.UseCors("AllowFrontend");

logger.LogInformation("Got here!");

app.MapGet("/hello", () => Results.Json(new { message = "Hello from server!" }));

app.MapGet("/users", async (AppDbContext db) => await db.Users.ToListAsync());

app.MapPost("/users", async (User user, AppDbContext db) =>
{
    db.Users.Add(user);
    await db.SaveChangesAsync();
    return Results.Created($"/users/{user.Id}", user);
});

app.Run();