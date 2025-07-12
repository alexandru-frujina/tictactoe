using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

var connStr = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connStr));

builder.WebHost.UseUrls("http://0.0.0.0:5000");

var jwtSecret = builder.Configuration["Jwt:Key"] ?? "secret-key-secret-key-secret-key";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "tic-tac-toe-app",
        ValidAudience = "tic-tac-toe-app",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();


// Automatically apply migrations, Useful for set-up fails early
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate(); // creates tables if they don't exist
}



var logger = app.Logger;

logger.LogInformation("Adding endpoints...");

app.MapPost("/signup", async (SignupDto dto, AppDbContext db) =>
{
    if (await db.Users.AnyAsync(u => u.Username == dto.Username))
        return Results.BadRequest(new { message = "Username already taken" });

    var user = new User
    {
        Username = dto.Username,
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
    };

    db.Users.Add(user);
    await db.SaveChangesAsync();

    var token = GenerateJwtToken(user);

    user.Connected = true;
    await db.SaveChangesAsync();

    return Results.Ok(new { token });
});

app.MapPost("/login", async (LoginDto dto, AppDbContext db) =>
{
    var user = await db.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
    if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        return Results.Unauthorized();

    var token = GenerateJwtToken(user);

    user.Connected = true;
    await db.SaveChangesAsync();

    return Results.Ok(new { token });
});

app.MapPost("/logout", [Authorize] async (HttpContext context, AppDbContext db) =>
{
    var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (userId != null)
    {
        var user = await db.Users.FindAsync(Int32.Parse(userId));
        if (user != null)
        {
            user.Connected = false;
            await db.SaveChangesAsync();
        }
    }

    return Results.Ok(new { message = "Logged out" });
});


app.MapGet("/me", [Authorize] (HttpContext context) =>
{
    var username = context.User.Identity?.Name;
    var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(userId))
        return Results.Unauthorized();

    return Results.Ok(new { username, userId });
});

app.MapGet("/connected-users", [Authorize] async (HttpContext context, AppDbContext db) =>
{
    var currentUserId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    var currentId = int.TryParse(currentUserId, out var id) ? id : -1;

    var connectedUsers = await db.Users
        .Where(u => u.Connected && u.Id != currentId)
        .Select(u => new { u.Id, u.Username })
        .ToListAsync();
    return Results.Ok(connectedUsers);
});

string GenerateJwtToken(User user)
{
    var claims = new[]
    {
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString())
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: "tic-tac-toe-app",
        audience: "tic-tac-toe-app",
        claims: claims,
        expires: DateTime.UtcNow.AddHours(1),
        signingCredentials: creds);

    return new JwtSecurityTokenHandler().WriteToken(token);
}

app.UseAuthentication();
app.UseAuthorization();

app.Run();