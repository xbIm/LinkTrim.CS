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

        private readonly IOptions<LinkTrimOptions> _options =
            Options.Create(new LinkTrimOptions() { Host = "http://test/" });

        [Fact]
        public async Task ShortenUrl_WithMockStorage_Successfully()
        {
            // Arrange
            var mockShorterStrategy = Substitute.For<IShortenerStrategy>();
            var logger = Substitute.For<ILogger<UrlShorteningService>>();
            var urlShorteningService = new UrlShorteningService(logger, _options, _mockStorage, mockShorterStrategy);

            var longUrl = FullUrl.Create("https://www.example.com/very-long-url-that-needs-shortening").Success;
            const string key = "abc123";

            _mockStorage.Exists(key).Returns(false);
            _mockStorage.SetUrl(key, longUrl.Value).Returns(Task.CompletedTask);
            mockShorterStrategy.ShortenUrl(longUrl, 0).Returns(key);

            // Act
            var result = await urlShorteningService.ShortenUrl(longUrl);

            // Assert
            Assert.Equal("http://test/" + key, result.Success.Value);
            await _mockStorage.Received().Exists(key);
            await _mockStorage.Received().SetUrl(key, longUrl.Value);
        }

        [Fact]
        public async Task GetOriginalUrl_Exists_Successfully()
        {
            // Arrange
            var mockShortenerStrategy = Substitute.For<IShortenerStrategy>();
            var logger = Substitute.For<ILogger<UrlShorteningService>>();

            var urlShorteningService = new UrlShorteningService(logger, _options, _mockStorage, mockShortenerStrategy);

            const string key = "abc123";
            const string longUrl = "https://www.example.com/very-long-url-that-needs-shortening";

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

            const string key = "abc123";

            _mockStorage.GetUrl(key).Returns(Option<string>.None);

            // Act
            var result = await urlShorteningService.GetOriginalUrl(key);

            // Assert
            Assert.Equal(result.Failure, result.Failure.AsT1);
            await _mockStorage.Received().GetUrl(key);
        }
    }
}