using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LinkTrim.Web.Models;
using Microsoft.Extensions.Options;

namespace LinkTrim.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IOptions<LinkTrimOptions> _options;

    public HomeController(ILogger<HomeController> logger, IOptions<LinkTrimOptions> options)
    {
        _logger = logger;
        _options = options;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        ViewBag.Host = _options.Value.Host;
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
