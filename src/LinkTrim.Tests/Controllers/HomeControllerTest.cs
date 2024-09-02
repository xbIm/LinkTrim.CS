using LinkTrim.Web;
using LinkTrim.Web.Controllers;
using LinkTrim.Web.Models;
using LinkTrim.Web.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace LinkTrim.Tests.Controllers;

public class HomeControllerTests
{
    [Fact]
    public async Task Index_ReturnsSuccessViewResult()
    {
        // Arrange
        var loggerMock = Substitute.For<ILogger<HomeController>>();
        var options = Options.Create(new LinkTrimOptions() { Host = "http://test/" });
        var service = Substitute.For<IUrlShorteningService>();
        var controller = new HomeController(loggerMock, options, service);
        var urlRequest = new UrlRequest(){ Value = "https://www.example.com/test-url" };
        var fullUrl = FullUrl.Create("http://test/short");
        service.ShortenUrl(Arg.Is<FullUrl>(e => e.Value == urlRequest.Value)).Returns(fullUrl);

        // Act
        var result = await controller.Index(urlRequest) as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Success", result.ViewName);
        Assert.Equal("http://test/short", (result.Model as FullUrl)!.Value);
    }
}