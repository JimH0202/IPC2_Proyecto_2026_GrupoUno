using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrbitNet.Web.Configuration;

var builder = WebApplication.CreateBuilder(args);

var portEnv = Environment.GetEnvironmentVariable("ASPNETCORE_PORT");
var port = string.IsNullOrWhiteSpace(portEnv) ? "5000" : portEnv;

var hemisphere = Environment.GetEnvironmentVariable("HEMISPHERE");
if (string.IsNullOrWhiteSpace(hemisphere))
{
    hemisphere = port == "5001" ? "South" : "North";
}

var hemisphereSettingsFile = $"appsettings.{hemisphere}.json";
builder.Configuration.AddJsonFile(hemisphereSettingsFile, optional: true, reloadOnChange: true);

builder.Services.AddControllersWithViews();

builder.Services.Configure<AppInstanceSettings>(builder.Configuration.GetSection("SystemConfiguration"));

builder.WebHost.UseUrls($"http://localhost:{port}");

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
