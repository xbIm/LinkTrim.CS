using LinkTrim.Web.Controllers;
using LinkTrim.Web;
using LinkTrim.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace LinkTrim.Tests.Controllers;

public class YourControllerTests
{
    [Fact]
    public async Task Index_ReturnsSuccessViewResult()
    {
        // Arrange
        var loggerMock = Substitute.For<ILogger<HomeController>>();
        var options = Options.Create(new LinkTrimOptions(){Host = ""});            
        var controller = new HomeController(loggerMock, options);
        var urlRequest = new UrlRequest("https://www.example.com/test-url"); 

        // Act
        var result = await controller.Index(urlRequest) as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Success", result.ViewName);  
    }
}

