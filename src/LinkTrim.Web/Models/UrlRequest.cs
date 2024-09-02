using System.ComponentModel.DataAnnotations;

namespace LinkTrim.Web.Models;

public class UrlRequest
{
    [RegularExpression(@"^(https:\/\/)?(www\.)?[a-zA-Z0-9-]+\.[a-zA-Z]{2,}/?[^\s]+", ErrorMessage = "Invalid URL")]
    [Required(ErrorMessage="Required")]
    [Display(Name = "Paste your URL into the box:")]
    public required string Value { get; init; }
}