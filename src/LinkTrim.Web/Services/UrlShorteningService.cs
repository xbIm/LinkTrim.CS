using LinkTrim.Web.Models;
using LinkTrim.Web.Services.Strategies;
using Microsoft.Extensions.Options;
using SimpleResult;

namespace LinkTrim.Web.Services;

public class UrlShorteningService: IUrlShorteningService
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

    public async Task<Result<string, Errors>> ShortenUrl(FullUrl longUrl)
    {
        var attempt = 0;
        while (attempt < _options.MaxAttempts)
        {
            var shortUrl = _shortenerStrategy.ShortenUrl(longUrl, attempt);
            if (await _storage.Exists(shortUrl))
            {
                attempt++;
            }
            else
            {
                await _storage.SetUrl(shortUrl, longUrl.Value);
                return shortUrl.ToSuccess<string, Errors>();
            }
        }

        return Result<string, Errors>.Failed(new ServerError("too many collisions"));
    }

    public async Task<Result<FullUrl, Errors>> GetOriginalUrl(string shortUrl)
    {
        var value = await _storage.GetUrl(shortUrl);
        if (!value.HasValue)
        {
            return Result<FullUrl, Errors>.Failed(new ShortCodeNotFound());
        }

        return FullUrl.Create(value.Value);
    }
}
