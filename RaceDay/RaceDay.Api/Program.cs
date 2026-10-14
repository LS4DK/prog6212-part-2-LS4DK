using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Database: EF Core talking to SQL Server via the connection string in appsettings.json
builder.Services.AddDbContext<RaceDayDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<PasswordService>();

// Server-side session: the server remembers UserId + Role, the browser only holds a session cookie
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.Name = ".RaceDay.Session";
    options.Cookie.HttpOnly = true;     // JavaScript can't read the cookie
    options.Cookie.IsEssential = true;
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
        options.SwaggerEndpoint("/openapi/v1.json", "RaceDay API v1"));
}

app.UseHttpsRedirection();
app.UseSession();          // must come before the controllers so they can read the session
app.UseAuthorization();
app.MapControllers();

app.Run();

// Lets the unit test project (Step 8) see the Program class
public partial class Program { }