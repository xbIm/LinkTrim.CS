using LinkTrim.Web;
using LinkTrim.Web.Services;
using LinkTrim.Web.Services.Strategies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.Configure<LinkTrimOptions>(builder.Configuration.GetSection("Options"));
builder.Services.AddSingleton<IStorage, UrlStorage>();
builder.Services.AddSingleton<IUrlShorteningService, UrlShorteningService>();
builder.Services.AddSingleton<IShortenerStrategy>(provider => new HashShortenerStrategy());

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
