using LinkTrim.Web.Models;
using LinkTrim.Web.Services.Strategies;

namespace LinkTrim.Tests.Strategies;

public class HashShortenerStrategyTests
{
    private readonly HashShortenerStrategy _strategy = new();

    [Theory]
    [InlineData("https://www.example.com/test-url")]
    [InlineData("https://ya.ru/")]
    [InlineData("https://www.google.com/search?q=long+url+example&oq=long+url+exm&gs_lcrp=EgZjaHJvbWUqCAgBEAAYFhgeMgYIABBFGDkyCAgBEAAYFhge0gEJNjUwNzZqMGo3qAIAsAIA&sourceid=chrome&ie=UTF-8#ip=1")]
    public void ShortenUrl_ReturnsCompactHash(string url)
    {
        // Arrange
        var validUrl = FullUrl.Create(url).Success;

        // Act
        var result = _strategy.ShortenUrl(validUrl, 0); // Assuming attempt value as 1 for testing

        // Assert
        Assert.NotNull(result);
        Assert.Equal(6, result.Length);
    }
}
