using SimpleResult;

namespace LinkTrim.Web.Models;

public record FullUrl
{
    public string Value { get; private set; }

    private FullUrl(string value)
    {
        Value = value;
    }

    public static Result<FullUrl, Errors> Create(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return Result<FullUrl, Errors>.Failed(new WrongFormat($"Name must not be empty: {nameof(value)})"));
        }

        if (Uri.TryCreate(value, UriKind.Absolute, out Uri? uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
        {
            var trimmedUrl = value.Trim();

            return Result<FullUrl, Errors>.Succeeded(new FullUrl(trimmedUrl));
        }

        return Result<FullUrl, Errors>.Failed(new WrongFormat("Invalid URL"));
    }
}