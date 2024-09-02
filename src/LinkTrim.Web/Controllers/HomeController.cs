using System.Diagnostics;

using LinkTrim.Web.Models;
using LinkTrim.Web.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using SimpleResult;
using SimpleResult.Extensions;

namespace LinkTrim.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IOptions<LinkTrimOptions> _options;
    private readonly IUrlShorteningService _urlShorteningService;

    public HomeController(ILogger<HomeController> logger, IOptions<LinkTrimOptions> options, IUrlShorteningService urlShorteningService)
    {
        _logger = logger;
        _options = options;
        _urlShorteningService = urlShorteningService;
    }

    [HttpGet("/")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost("/")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(UrlRequest request)
    {
        if (request.Value == null)
        {
            throw new ArgumentNullException(nameof(request), "Request.Value is null");
        }

        _logger.LogDebug("text: {Request}", request.Value);

        var result = await FullUrl.Create(request.Value)
            .BindAsync(_urlShorteningService.ShortenUrl);

        return result.IsSuccess ?
            View("Success", result.Success) :
            MapError(result.Failure);
    }

    [Route("/{shortUrl}")]
    public async Task<IActionResult> GetUrl(string shortUrl)
    {
        var result = await _urlShorteningService.GetOriginalUrl(shortUrl);
        return result.IsSuccess ?
            Redirect(result.Success.Value) :
            MapError(result.Failure);
    }

    [HttpGet]
    public IActionResult Information()
    {
        ViewBag.Host = _options.Value.Host;
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(NewError("error"));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult CodeNotFound()
    {
        Response.StatusCode = 400;
        return View("CodeNotFound");
    }

    private IActionResult MapError(Errors error)
    {
        return error.Match(
            _ => View("Error", NewError("Please enter proper Url")),
            _ => CodeNotFound(),
            serverError => View("Error", NewError(serverError.Text)));
    }

    private ErrorViewModel NewError(string errorText)
    {
        return new ErrorViewModel(errorText) { RequestId = HttpContext.TraceIdentifier };
    }
}