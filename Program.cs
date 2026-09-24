using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using WerewolfGM.Web.Components;
using WerewolfGM.Web.Data;
using WerewolfGM.Web.Hubs;
using WerewolfGM.Web.Services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080"; 
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// ---------- Base de données (SQLite, un seul fichier) ----------
var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? "Data Source=werewolf.db";
builder.Services.AddDbContextFactory<AppDbContext>(options => options.UseSqlite(connectionString));

// ---------- Services métier ----------
builder.Services.AddScoped<PlayerService>();
builder.Services.AddScoped<ChatService>();
builder.Services.AddScoped<VoteService>();

// ---------- Authentification par cookie (login = code personnel) ----------
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/login";
        options.ExpireTimeSpan = TimeSpan.FromDays(14);
        options.SlidingExpiration = true;
    });
builder.Services.AddAuthorization(options =>
{

    options.AddPolicy("GameMaster", policy => policy.RequireClaim("IsGameMaster", "true"));
});
builder.Services.AddCascadingAuthenticationState();

// ---------- Blazor + SignalR ----------
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddSignalR();

var app = builder.Build();

// ---------- Seed de la base au démarrage ----------
using (var scope = app.Services.CreateScope())
{
    var dbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
    using var db = dbFactory.CreateDbContext();
    DbInitializer.Initialize(db);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

// ---------- Endpoints d'authentification (code d'accès) ----------
app.MapPost("/api/login", async (
    HttpContext http,
    PlayerService players,
    [FromForm] string accessCode,
    [FromForm] string? returnUrl) =>
{
    var player = await players.FindByAccessCodeAsync(accessCode ?? string.Empty);
    if (player is null)
    {
        return Results.Redirect("/login?error=1");
    }

    var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, player.Id.ToString()),
        new(ClaimTypes.Name, player.FullName),
        new("PlayerId", player.Id.ToString()),
        new("IsGameMaster", player.IsGameMaster ? "true" : "false")
    };
    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    await http.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

    var target = player.IsGameMaster ? "/mj/tableau-de-bord" : "/accueil";
    return Results.Redirect(string.IsNullOrWhiteSpace(returnUrl) ? target : returnUrl);
}).AllowAnonymous().DisableAntiforgery();

app.MapPost("/api/logout", async (HttpContext http) =>
{
    await http.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/login");
}).AllowAnonymous();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapHub<GameHub>("/hubs/game");

app.Run();