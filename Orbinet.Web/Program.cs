using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrbitNet.Web.Configuration;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

var portEnv = Environment.GetEnvironmentVariable("ASPNETCORE_PORT");
var urlsEnv = Environment.GetEnvironmentVariable("ASPNETCORE_URLS");
var port = string.Empty;

if (!string.IsNullOrWhiteSpace(portEnv))
{
    port = portEnv;
}
else if (!string.IsNullOrWhiteSpace(urlsEnv))
{
    var firstUrl = urlsEnv.Split(';', StringSplitOptions.RemoveEmptyEntries)[0].Trim();
    if (Uri.TryCreate(firstUrl, UriKind.Absolute, out var parsedUrl))
    {
        port = parsedUrl.Port.ToString();
    }
}

if (string.IsNullOrWhiteSpace(port))
{
    port = "5000";
}

var hemisphere = Environment.GetEnvironmentVariable("HEMISPHERE");
if (string.IsNullOrWhiteSpace(hemisphere))
{
    hemisphere = port == "5001" ? "South" : "North";
}

var hemisphereSettingsFile = $"appsettings.{hemisphere}.json";
builder.Configuration.AddJsonFile(hemisphereSettingsFile, optional: true, reloadOnChange: true);

// Add localization services
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddControllersWithViews()
    .AddDataAnnotationsLocalization()
    .AddViewLocalization();

builder.Services.Configure<AppInstanceSettings>(builder.Configuration.GetSection("SystemConfiguration"));

builder.WebHost.UseUrls($"http://localhost:{port}");

var app = builder.Build();

// Configure localization
var supportedCultures = new[] { "es", "en" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture("es")
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

// Add providers for localization (order matters - first match wins)
localizationOptions.RequestCultureProviders.Clear();
localizationOptions.RequestCultureProviders.Add(new Microsoft.AspNetCore.Localization.CookieRequestCultureProvider { CookieName = ".AspNetCore.Culture" });
localizationOptions.RequestCultureProviders.Add(new Microsoft.AspNetCore.Localization.QueryStringRequestCultureProvider());
localizationOptions.RequestCultureProviders.Add(new Microsoft.AspNetCore.Localization.AcceptLanguageHeaderRequestCultureProvider());

app.UseRequestLocalization(localizationOptions);

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
