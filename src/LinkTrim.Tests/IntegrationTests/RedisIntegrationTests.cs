using LinkTrim.Web;
using LinkTrim.Web.Services;

using Microsoft.Extensions.Options;

using StackExchange.Redis;

using Testcontainers.Redis;

namespace LinkTrim.Tests.IntegrationTests
{
    public class RedisIntegrationTests : IAsyncLifetime
    {
        private IConnectionMultiplexer _redisConnection = null!;
        private IStorage _storage = null!;

        [Fact]
        [Trait("Category", "Integration")]
        public async Task PingRedis_Successfully()
        {
            // Act
            var server = _redisConnection.GetServer(_redisConnection.GetEndPoints()[0]);
            var result = await server.PingAsync();

            // Assert
            Assert.True(result.Microseconds > 0);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task SetAndGetKey_Successfully()
        {
            // Arrange
            var database = _redisConnection.GetDatabase();
            const string key = "test_key";
            const string expectedValue = "test_value";

            // Act
            await database.StringSetAsync(key, expectedValue);
            string? retrievedValue = await database.StringGetAsync(key);

            // Assert
            Assert.Equal(expectedValue, retrievedValue);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task SetLongUrl_Successfully()
        {
            // Arrange
            const string shortUrl = "abcde";
            const string longurl = "https://example.com";

            // Act
            await _storage.SetUrl(shortUrl, longurl);
            var exists = await _storage.Exists(shortUrl);
            var existsValue = await _storage.ExistsValue(longurl);
            var result = await _storage.GetUrl(shortUrl);

            // Assert
            Assert.True(exists);
            Assert.True(existsValue.HasValue);
            Assert.Equal(longurl, result.Value);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task RemoveUrl_KeyExists_RemovesKeyAndIndex()
        {
            // Arrange
            const string shortUrl = "abcde";
            const string longurl = "https://example.com";
            await _storage.SetUrl(shortUrl, longurl);

            // Act
            await _storage.Remove(shortUrl);
            var keyExists = await _storage.Exists(shortUrl);
            var valueExists = await _storage.ExistsValue(longurl);

            // Assert
            Assert.False(keyExists);
            Assert.False(valueExists.HasValue);
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
            _storage = new UrlStorage(_redisConnection, new OptionsWrapper<LinkTrimOptions>(new LinkTrimOptions { Host = "http://localhost:5000" }));
        }

        public Task DisposeAsync()
        {
            _redisConnection.Dispose();
            return Task.CompletedTask;
        }
    }
}