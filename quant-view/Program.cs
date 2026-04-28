using Microsoft.EntityFrameworkCore;
using RiskNotes.Data;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    if(!string.IsNullOrWhiteSpace(databaseUrl))
    {
        string connectionString;

        if (databaseUrl.StartsWith("postgres://") || 
            databaseUrl.StartsWith("postgresql://"))
        {
            var uri = new Uri(databaseUrl);
            var userInfo = uri.UserInfo.Split(':', 2);

            var username = Uri.UnescapeDataString(userInfo[0]);
            var password = Uri.UnescapeDataString(userInfo[1]);
            var database = uri.AbsolutePath.TrimStart('/');

            connectionString = 
                $"Host={uri.Host};" +
                $"Port={uri.Port};" +
                $"Database={database};" +
                $"Username={username};" +
                $"Password={password};" +
                $"SSL Mode=Require;" +
                $"Trust Server Certificate=true";
        } else
        {
            connectionString = databaseUrl
                .Replace("Host-tcp://", "Host=")
                .Replace("Server=tcp://", "Server=");
        }
        options.UseNpgsql(connectionString);
    } else
    {        
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
    }
});

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<AppDbContext>();

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

var app = builder.Build();

using(var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages();

app.Run();
