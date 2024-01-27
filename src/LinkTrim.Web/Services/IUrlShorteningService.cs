using LinkTrim.Web.Models;
using SimpleResult;

namespace LinkTrim.Web.Services;

public interface IUrlShorteningService
{
    Task<Result<string, Errors>> ShortenUrl(FullUrl longUrl);

    Task<Result<FullUrl, Errors>> GetOriginalUrl(string shortUrl);
}
