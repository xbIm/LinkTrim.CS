using LinkTrim.Web.Models;
using LinkTrim.Web.Services.Strategies;

using Microsoft.Extensions.Options;

using SerilogTimings;

using SimpleResult;

namespace LinkTrim.Web.Services;

public class UrlShorteningService : IUrlShorteningService
{
    private readonly ILogger<UrlShorteningService> _logger;
    private readonly LinkTrimOptions _options;
    private readonly IStorage _storage;
    private readonly IShortenerStrategy _shortenerStrategy;

    public UrlShorteningService(
        ILogger<UrlShorteningService> logger,
        IOptions<LinkTrimOptions> options,
        IStorage storage,
        IShortenerStrategy shortenerStrategy)
    {
        _logger = logger;
        _options = options.Value;
        _storage = storage;
        _shortenerStrategy = shortenerStrategy;
    }

    public async Task<Result<FullUrl, Errors>> ShortenUrl(FullUrl longUrl)
    {
        var valueExists = await _storage.ExistsValue(longUrl.Value);
        if (valueExists.HasValue)
        {
            return ToFullUrl(valueExists.Value);
        }

        var attempt = 0;
        using (var op = Operation.Begin("Generate short url for {LongUrl}", longUrl.Value))
        {
            while (attempt < _options.MaxAttempts)
            {
                var shortUrl = _shortenerStrategy.ShortenUrl(longUrl, attempt);
                if (await _storage.Exists(shortUrl))
                {
                    attempt++;
                    continue;
                }

                using (Operation.Time("Save url {ShortUrl} with {LongUrl} attempt:{Attempt}",
                           shortUrl,
                           longUrl.Value,
                           attempt))
                {
                    op.Complete();
                    await _storage.SetUrl(shortUrl, longUrl.Value);
                    return ToFullUrl(shortUrl);
                }
            }
        }

        return Result<FullUrl, Errors>.Failed(new ServerError("too many collisions"));
    }

    public async Task<Result<FullUrl, Errors>> GetOriginalUrl(string shortUrl)
    {
        using (Operation.Time("Get Original Url for {ShortUrl}", shortUrl))
        {
            var value = await _storage.GetUrl(shortUrl);
            return !value.HasValue ?
                Result<FullUrl, Errors>.Failed(new ShortCodeNotFound()) :
                FullUrl.Create(value.Value);
        }
    }

    private Result<FullUrl, Errors> ToFullUrl(string shortCode)
    {
        return FullUrl.Create(_options.Host + shortCode);
    }
}