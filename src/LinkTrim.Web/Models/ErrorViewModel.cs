namespace LinkTrim.Web.Models;

public class ErrorViewModel(string errorText)
{
    public string? RequestId { get; set; }

    public string ErrorText { get; set; } = errorText;

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}