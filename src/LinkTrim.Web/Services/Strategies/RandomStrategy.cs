using LinkTrim.Web.Models;

namespace LinkTrim.Web.Services.Strategies;


public class RandomShortenerStrategy : IShortenerStrategy
{
    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    public string ShortenUrl(FullUrl url, int attempt)
    {
        // Use the URL value as a seed for the random string
        int seed = url.Value.GetHashCode() ^ attempt; // Incorporate the attempt for additional variation
        Random random = new Random(seed);

        // todo: add parameter
        char[] stringChars = new char[6];
        for (int i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = chars[random.Next(chars.Length)];
        }
        return new string(stringChars);
    }
}
