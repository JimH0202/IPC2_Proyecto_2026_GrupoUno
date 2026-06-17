using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orbinet.Web.Configuration;
using Orbinet.Web.Services.Communication;     // Para que encuentre RelayHttpService
using Orbinet.Web.Services.SimulationEngine;  // Para que encuentre TickProcessor

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
builder.Services.AddControllersWithViews(); 
builder.Services.AddHttpClient(); // Requisito para comunicación entre puertos

// 2. Inyección de tus servicios y configuraciones del sistema
builder.Services.Configure<AppInstanceSettings>(builder.Configuration.GetSection("SystemConfiguration"));
builder.Services.AddSingleton<OrbitNetStore>();
builder.Services.AddSingleton<BasicAuthService>();
builder.Services.AddSingleton<XmlIngestService>();
builder.Services.AddSingleton<RelayHttpService>();
builder.Services.AddSingleton<TickProcessor>();

var app = builder.Build();

// 3. Configuración del Pipeline HTTP (Rutas y Archivos Estáticos)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Soporte para CSS, JS e imágenes del frontend
app.UseRouting();
app.UseAuthorization();

// 4. Mapeo de rutas para Vistas y Endpoints API REST unificado
app.MapControllers(); 
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();