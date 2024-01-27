using LinkTrim.Web.Models;

namespace LinkTrim.Web.Services.Strategies;

public interface IShortenerStrategy
{
    string ShortenUrl(FullUrl url, int attempt);
}
