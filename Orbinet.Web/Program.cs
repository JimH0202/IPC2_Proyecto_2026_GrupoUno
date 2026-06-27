// 3. Configuración del Pipeline HTTP (Rutas y Seguridad)
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orbinet.Web.Configuration;
using Orbinet.Web.Services;
using Orbinet.Web.Services.Communication;     // Para que encuentre RelayHttpService
using Orbinet.Web.Services.SimulationEngine;  // Para que encuentre TickProcessor
using Orbinet.Web.Models.Entities; // Para asegurar la comunicación con la raíz de tus modelos y entidades

var builder = WebApplication.CreateBuilder(args);

// --- CONFIGURACIÓN DINÁMICA DE PUERTOS Y HEMISFERIOS
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
builder.Services.AddControllersWithViews(); // Soporta Vistas Razor y Endpoints de API
builder.Services.AddHttpClient(); // Requisito para comunicación entre puertos

// 2. Inyección de tus servicios y configuraciones del sistema
builder.Services.Configure<AppInstanceSettings>(builder.Configuration.GetSection("SystemConfiguration"));
builder.Services.AddSingleton<OrbitNetStore>();
builder.Services.AddSingleton<BasicAuthService>();
builder.Services.AddSingleton<XmlIngestService>();
builder.Services.AddSingleton<RelayHttpService>();
builder.Services.AddSingleton<TickProcessor>();
builder.Services.AddSingleton<OrbitalRotator>();
builder.Services.AddSingleton<PriorityDispatcher>();
builder.Services.AddSingleton<RoutingService>();
builder.Services.AddSingleton<SimulationCoordinator>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
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
