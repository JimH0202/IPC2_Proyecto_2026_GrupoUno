var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpClient();

builder.Services.Configure<AppInstanceSettings>(builder.Configuration.GetSection("AppInstance"));

builder.Services.AddSingleton<OrbitNetStore>();
builder.Services.AddSingleton<BasicAuthService>();
builder.Services.AddSingleton<XmlIngestService>();
builder.Services.AddSingleton<RelayHttpService>();
builder.Services.AddSingleton<TickProcessor>();

var app = builder.Build();

app.MapControllers();

app.Run();
