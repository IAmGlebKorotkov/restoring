using Microsoft.AspNetCore.Authentication.Cookies;
using Restoran.AdminPanel.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(24);
        options.SlidingExpiration = true;
    });

var authUrl = builder.Configuration["Services:AuthUrl"] ?? "http://localhost:5001";
var backendUrl = builder.Configuration["Services:BackendUrl"] ?? "http://localhost:5002";

builder.Services.AddHttpClient<AuthApiService>(c => c.BaseAddress = new Uri(authUrl));
builder.Services.AddHttpClient<BackendApiService>(c => c.BaseAddress = new Uri(backendUrl));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
