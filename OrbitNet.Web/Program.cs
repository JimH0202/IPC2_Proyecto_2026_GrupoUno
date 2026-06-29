using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using System;
using OrbitNet.Web.Configuration;
using OrbitNet.Web.Services;
using OrbitNet.Web.Services.Communication;
using OrbitNet.Web.Services.SimulationEngine;
using OrbitNet.Web.Models.Entities;

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

builder.WebHost.UseUrls($"http://localhost:{port}");
// ------------------------------------------------------------------------

// 1. Configuraciones compartidas (MVC + API)
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddControllersWithViews()
    .AddDataAnnotationsLocalization()
    .AddViewLocalization(); // Soporta Vistas Razor y Endpoints de API
builder.Services.AddHttpClient(); // Requisito para comunicación entre puertos

// 2. Inyección de tus servicios y configuraciones del sistema
// IMPORTANTE: Verifica que tu appsettings.json use "SystemConfiguration" en lugar de "AppInstance"
builder.Services.Configure<AppInstanceSettings>(builder.Configuration.GetSection("AppInstance"));
builder.Services.AddSingleton<OrbitNetStore>();
builder.Services.AddSingleton<BasicAuthService>();
builder.Services.AddSingleton<XmlIngestService>();
builder.Services.AddSingleton<RelayHttpService>();
        builder.Services.AddSingleton<TickProcessor>();
builder.Services.AddSingleton<OrbitalRotator>();
builder.Services.AddSingleton<RoutingService>();
builder.Services.AddSingleton<PriorityDispatcher>();
builder.Services.AddSingleton<SimulationCoordinator>();
builder.Services.AddSingleton<OrbitNetDataService>();

var app = builder.Build();

// 3. Configuración del Pipeline HTTP (Rutas y Archivos Estáticos)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}

// Configurar localización (idioma)
var supportedCultures = new[] { "es", "en" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture("es")
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

localizationOptions.RequestCultureProviders.Clear();
localizationOptions.RequestCultureProviders.Add(new Microsoft.AspNetCore.Localization.CookieRequestCultureProvider { CookieName = ".AspNetCore.Culture" });
localizationOptions.RequestCultureProviders.Add(new Microsoft.AspNetCore.Localization.QueryStringRequestCultureProvider());
localizationOptions.RequestCultureProviders.Add(new Microsoft.AspNetCore.Localization.AcceptLanguageHeaderRequestCultureProvider());

app.UseRequestLocalization(localizationOptions);

app.UseStaticFiles(); // Soporte para CSS, JS e imágenes del frontend
app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets();

// 4. Mapeo de rutas para Vistas y Endpoints API REST unificado
app.MapControllers(); // Mapea tus rutas como /api/v1/space/...
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();

public partial class Program { }
