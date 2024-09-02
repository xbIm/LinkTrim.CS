using System.Globalization;
using System.Security.Cryptography;
using System.Text;

using LinkTrim.Web.Models;

namespace LinkTrim.Web.Services.Strategies;

public class HashShortenerStrategy : IShortenerStrategy
{
    public string ShortenUrl(FullUrl url, int attempt)
    {
        // Combine the URL value with the attempt for uniqueness
        string combinedInput = url.Value + attempt;

        byte[] inputBytes = Encoding.UTF8.GetBytes(combinedInput);

        #pragma warning disable CA5351
        byte[] hashBytes = MD5.HashData(inputBytes.AsSpan());
        #pragma warning restore CA5351

        var sb = new StringBuilder();
        for (int i = 0; i < 4; i++) // We take only first 3 bytes to get 6 hexadecimal characters
        {
            sb.Append(hashBytes[i].ToString("X2", CultureInfo.InvariantCulture));
        }

        string result = ConvertHexToCustomFormat(
            sb.ToString(),
            "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789");
        return result.PadLeft(6, '0');
    }

    private static string ConvertHexToCustomFormat(string hexString, string customChars)
    {
        // Convert the hexadecimal string to a decimal value
        long decimalValue = long.Parse(hexString, System.Globalization.NumberStyles.HexNumber);

        // Determine the length of the custom character set
        int baseValue = customChars.Length;

        // Perform custom base conversion
        var result = string.Empty;
        while (decimalValue > 0)
        {
            result = customChars[(int)(decimalValue % baseValue)] + result;
            decimalValue /= baseValue;
        }

        return result;
    }
}