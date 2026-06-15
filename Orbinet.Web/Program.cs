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

// 3. Configuración del Pipeline HTTP (Rutas y Seguridad)
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