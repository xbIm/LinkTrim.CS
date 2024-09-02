using LinkTrim.Web.Models;

namespace LinkTrim.Web.Services.Strategies;

public class RandomShortenerStrategy : IShortenerStrategy
{
    const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    private const ushort ShorlUrlLength = 6;

    public string ShortenUrl(FullUrl url, int attempt)
    {
        // Use the URL value as a seed for the random string
        var seed = url.Value.GetHashCode() ^ attempt; // Incorporate the attempt for additional variation
        var random = new Random(seed);

        char[] stringChars = new char[ShorlUrlLength];
        for (int i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = Chars[random.Next(Chars.Length)];
        }
        return new string(stringChars);
    }
}