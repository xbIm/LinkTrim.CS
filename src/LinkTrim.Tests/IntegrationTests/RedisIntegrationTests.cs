using LinkTrim.Web.Services;
using StackExchange.Redis;
using Testcontainers.Redis;

namespace LinkTrim.Tests.IntegrationTests
{
    public class RedisIntegrationTests : IAsyncLifetime
    {
        private IConnectionMultiplexer _redisConnection = null!;
        private IStorage _storage = null!;

        [Fact]
        public async Task PingRedis_Successfully()
        {
            // Arrange (already done in InitializeAsync)

            // Act
            var server = _redisConnection.GetServer(_redisConnection.GetEndPoints()[0]);
            var result = await server.PingAsync();

            // Assert
            Assert.True(result.Microseconds > 0);
        }

        [Fact]
        public async Task SetAndGetKey_Successfully()
        {
            // Arrange
            var database = _redisConnection.GetDatabase();
            string key = "test_key";
            string expectedValue = "test_value";

            // Act
            await database.StringSetAsync(key, expectedValue);
            string? retrievedValue = await database.StringGetAsync(key);

            // Assert
            Assert.Equal(expectedValue, retrievedValue);
        }

        [Fact]
        public async Task SetLongUrl_Successfully()
        {
            // Arrange
            var shortUrl = "abcde";
            var longurl = "https://example.com";

            // Act
            await _storage.SetUrl(shortUrl, longurl);
            var exists = await _storage.Exists(longurl);
            var result = await _storage.GetUrl(shortUrl);

            // Assert
            Assert.True(exists);
            Assert.Equal(longurl, result.Value);
        }

        [Fact]
        public async Task RemoveUrl_KeyExists_RemovesKeyAndIndex()
        {
            // Arrange
            var shortUrl = "abcde";
            var longurl = "https://example.com";
            await _storage.SetUrl(shortUrl, longurl);

            // Act
            await _storage.Remove(shortUrl);

            // Assert
            var valueExists = await _storage.Exists(longurl);
            var keyExists = await _storage.GetUrl(shortUrl);
            Assert.False(keyExists.HasValue);
            Assert.False(valueExists);
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
            _storage = new UrlStorage(_redisConnection);
        }

        public Task DisposeAsync()
        {
            _redisConnection.Dispose();
            return Task.CompletedTask;
        }
    }
}
