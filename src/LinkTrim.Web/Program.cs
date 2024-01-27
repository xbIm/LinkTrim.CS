using LinkTrim.Web;
using LinkTrim.Web.Services;
using LinkTrim.Web.Services.Strategies;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.Configure<LinkTrimOptions>(builder.Configuration.GetSection("Options"));
builder.Services.AddSingleton<IStorage, UrlStorage>();
builder.Services.AddSingleton<IUrlShorteningService, UrlShorteningService>();
builder.Services.AddSingleton<IShortenerStrategy>(provider => new HashShortenerStrategy());
builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
    ConnectionMultiplexer.Connect(Environment.GetEnvironmentVariable("REDIS_CONNECTION")!));
builder.Services.AddScoped<IDatabase>(services => services.GetService<IConnectionMultiplexer>()!.GetDatabase());
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
