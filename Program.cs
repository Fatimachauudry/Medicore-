using MediCore.Data;

var builder = WebApplication.CreateBuilder(args);

// ── Services ──────────────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddSingleton<MediCoreDb>();   // single in-memory DB instance

// Session (cookie-based, server-side)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(opts =>
{
    opts.IdleTimeout        = TimeSpan.FromHours(4);
    opts.Cookie.HttpOnly    = true;
    opts.Cookie.IsEssential = true;
    opts.Cookie.SameSite    = SameSiteMode.Strict;
});

var app = builder.Build();

// ── Middleware ────────────────────────────────────────────────────────────
app.UseStaticFiles();     // serves wwwroot/
app.UseSession();
app.MapControllers();

// Serve index.html for the SPA root
app.MapFallbackToFile("index.html");

app.Run();
