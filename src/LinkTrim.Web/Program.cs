using LinkTrim.Web;
using LinkTrim.Web.Middleware;
using LinkTrim.Web.Services;
using LinkTrim.Web.Services.Strategies;

using Serilog;
using Serilog.Templates;

using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.Configure<LinkTrimOptions>(builder.Configuration.GetSection("Options"));

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<HttpRequestTraceIdEnricher>();

builder.Services.AddSingleton<IStorage, UrlStorage>();
builder.Services.AddSingleton<IUrlShorteningService, UrlShorteningService>();
builder.Services.AddSingleton<IShortenerStrategy>(_ => new HashShortenerStrategy());
builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
    ConnectionMultiplexer.Connect(Environment.GetEnvironmentVariable("REDIS_CONNECTION")!));
builder.Services.AddScoped<IDatabase>(services => services.GetService<IConnectionMultiplexer>()!.GetDatabase());

builder.Host.UseSerilog();

var app = builder.Build();

var loggerConfigureation = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.With(app.Services.GetService<HttpRequestTraceIdEnricher>()!)
    .Enrich.With(new RemovePropertiesEnricher(["PathBase", "Scheme"]));

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    loggerConfigureation = loggerConfigureation.WriteTo.Console(new ExpressionTemplate(
        "{ {timestamp: @t, message: @m, level: @l, exception: @x, ..@p} }\n"));
}
else
{
    loggerConfigureation = loggerConfigureation.WriteTo.Console();
}
Log.Logger = loggerConfigureation.CreateLogger();

app.UseMiddleware<TraceIdMiddleware>();

app.UseStaticFiles();

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

public partial class Program;