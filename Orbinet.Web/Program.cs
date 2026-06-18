// 3. Configuración del Pipeline HTTP (Rutas y Seguridad)
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orbinet.Web.Configuration;
using Orbinet.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuraciones compartidas (MVC + API)
builder.Services.AddControllersWithViews(); // Soporta Vistas Razor y Endpoints de API
builder.Services.AddHttpClient(); // Requisito para comunicación entre puertos

// 2. Inyección de tus servicios (TDAs, Motor y Comunicación)
builder.Services.Configure<AppInstanceSettings>(builder.Configuration.GetSection("AppInstance"));
builder.Services.AddSingleton<OrbitNetStore>();
builder.Services.AddSingleton<BasicAuthService>();
builder.Services.AddSingleton<XmlIngestService>();
builder.Services.AddSingleton<RelayHttpService>();
builder.Services.AddSingleton<TickProcessor>();

var app = builder.Build();

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

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();
app.MapStaticAssets();

// 4. Mapeo de rutas para Vistas y Endpoints API REST
app.MapControllers(); // Mapea tus rutas como /api/v1/space/...
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();