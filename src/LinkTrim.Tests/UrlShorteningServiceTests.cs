using LinkTrim.Web;
using LinkTrim.Web.Models;
using LinkTrim.Web.Services;
using LinkTrim.Web.Services.Strategies;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using SimpleResult;

namespace LinkTrim.Tests
{
    public class UrlShorteningServiceTests
    {
        private readonly IStorage _mockStorage = Substitute.For<IStorage>();
        private readonly IOptions<LinkTrimOptions> _options = Substitute.For<IOptions<LinkTrimOptions>>();

        public UrlShorteningServiceTests()
        {
            _options.Value.Returns(new LinkTrimOptions() { Host = "", MaxAttempts = 10 });
        }


        [Fact]
        public async Task ShortenUrl_WithMockStorage_Successfully()
        {
            // Arrange
            var mockShorterStrategy = Substitute.For<IShortenerStrategy>();
            var logger = Substitute.For<ILogger<UrlShorteningService>>();

            var urlShorteningService = new UrlShorteningService(logger, _options,  _mockStorage, mockShorterStrategy);

            var longUrl =  FullUrl.Create("https://www.example.com/very-long-url-that-needs-shortening").Success;
            string key = "abc123";
            //todo: decide which level add short.url
            string shortUrl = "http://short.url/" + key;

            _mockStorage.Exists(key).Returns(false);
            _mockStorage.SetUrl(key, longUrl.Value).Returns(Task.CompletedTask);
            mockShorterStrategy.ShortenUrl(longUrl, 0).Returns(key);

            // Act
            var result = await urlShorteningService.ShortenUrl(longUrl);

            // Assert
            Assert.Equal(key, result.Success);
            await _mockStorage.Received().Exists(key);
            await _mockStorage.Received().SetUrl(key, longUrl.Value);
        }

        [Fact]
        public async Task GetOriginalUrl_Exists_Successfully()
        {
            // Arrange
            var mockShortenerStrategy = Substitute.For<IShortenerStrategy>();
            var logger = Substitute.For<ILogger<UrlShorteningService>>();
            var options = Substitute.For<IOptions<LinkTrimOptions>>();

            var urlShorteningService = new UrlShorteningService(logger, options, _mockStorage, mockShortenerStrategy);

            var key = "abc123";
            var longUrl = "https://www.example.com/very-long-url-that-needs-shortening";

            _mockStorage.GetUrl(key).Returns(longUrl.ToOption());

            // Act
            var result = await urlShorteningService.GetOriginalUrl(key);

            // Assert
            Assert.Equal(longUrl, result.Success.Value);
            await _mockStorage.Received().GetUrl(key);
        }

        [Fact]
        public async Task GetOriginalUrl_Missing_Successfully()
        {
            // Arrange
            var mockShortenerStrategy = Substitute.For<IShortenerStrategy>();
            var logger = Substitute.For<ILogger<UrlShorteningService>>();

            var urlShorteningService = new UrlShorteningService(logger, _options, _mockStorage, mockShortenerStrategy);

            var key = "abc123";

            _mockStorage.GetUrl(key).Returns(Option<string>.None);

            // Act
            var result = await urlShorteningService.GetOriginalUrl(key);

            // Assert
            Assert.Equal(result.Failure, result.Failure.AsT1);
            await _mockStorage.Received().GetUrl(key);
        }
    }
}
