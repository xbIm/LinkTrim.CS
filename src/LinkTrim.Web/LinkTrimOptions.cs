namespace LinkTrim.Web;

public class LinkTrimOptions
{
    public required string Host { get; init; }

    public int MaxAttempts { get; init; } = 10;

    public TimeSpan Expired { get; init; } = TimeSpan.FromDays(365);
}