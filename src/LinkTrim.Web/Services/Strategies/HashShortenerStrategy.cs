using System;
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

        using var algorithm = MD5.Create();
        byte[] inputBytes = Encoding.UTF8.GetBytes(combinedInput);
        byte[] hashBytes = algorithm.ComputeHash(inputBytes);

        // Convert the byte array to a hexadecimal string
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < 4; i++) // We take only first 3 bytes to get 6 hexadecimal characters
        {
            sb.Append(hashBytes[i].ToString("X2"));
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
        string result = "";
        while (decimalValue > 0)
        {
            result = customChars[(int)(decimalValue % baseValue)] + result;
            decimalValue /= baseValue;
        }

        return result;
    }

}
