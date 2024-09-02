using LinkTrim.Web;
using LinkTrim.Web.Models;
using LinkTrim.Web.Services;
using LinkTrim.Web.Services.Strategies;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using NSubstitute;

using SimpleResult;
using StackExchange.Redis;

using Testcontainers.Redis;

namespace LinkTrim.Tests
{
    public class UrlShorteningServiceIntegrationTests : IAsyncLifetime
    {
        private readonly IOptions<LinkTrimOptions> _options =
            Options.Create(new LinkTrimOptions() { Host = "http://test/" });

        private readonly ILogger<UrlShorteningService> _logger = Substitute.For<ILogger<UrlShorteningService>>();
        private IConnectionMultiplexer _redisConnection = null!;
        private IStorage _redisStorage = null!;
        private UrlShorteningService _urlShorteningService = null!;

        [Fact]
        public async Task ShortenUrl_WithRedisStorage_Successfully()
        {
            // Arrange
            var redisDb = _redisConnection.GetDatabase();

            var longUrl = FullUrl.Create("https://www.example.com/very-long-url-that-needs-shortening").Success;

            // Act
            var keyResult = await _urlShorteningService.ShortenUrl(longUrl);

            // Assert
            var shortKey = keyResult.Success.Value.Replace("http://test/", "");
            Assert.Equal(
                longUrl.Value,
                await redisDb.HashGetAsync(shortKey, "longUrl")
                );
        }

        [Fact]
        public async Task GetOriginalUrl_Exists_Successfully()
        {
            // Arrange
            var redisDb = _redisConnection.GetDatabase();

            const string key = "abc123";
            const string longUrl = "https://www.example.com/very-long-url-that-needs-shortening";

            // Prepopulate Redis
            await redisDb.HashSetAsync(key, "longUrl", longUrl);
            await redisDb.SetAddAsync("value:"+ longUrl, key);

            // Act
            var result = await _urlShorteningService.GetOriginalUrl(key);

            // Assert
            Assert.Equal(longUrl, result.Success.Value);
        }

        [Fact]
        public async Task GetOriginalUrl_Missing_Successfully()
        {
            // Arrange
            const string key = "abc123";

            // Act
            var result = await _urlShorteningService.GetOriginalUrl(key);

            // Assert
            Assert.False(result.IsSuccess);
        }

        public async Task InitializeAsync()
        {
            var redisContainer = new RedisBuilder()
                .WithImage("redis:7.0")
                .Build();

            await redisContainer.StartAsync();

            var connectionString = redisContainer.GetConnectionString();

            await redisContainer.StartAsync();

            _redisConnection = await ConnectionMultiplexer.ConnectAsync(connectionString);
            _redisStorage = new UrlStorage(_redisConnection, new OptionsWrapper<LinkTrimOptions>(new LinkTrimOptions { Host = "http://localhost:5000" }));
            var mockShortenerStrategy = Substitute.For<IShortenerStrategy>();
            _urlShorteningService = new UrlShorteningService(_logger, _options, _redisStorage, new HashShortenerStrategy());
        }

        public Task DisposeAsync()
        {
            _redisConnection.Dispose();
            return Task.CompletedTask;
        }
    }
}