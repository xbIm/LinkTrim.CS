using OneOf;

namespace LinkTrim.Web.Models;

public record WrongFormat(string Text);

public record ShortCodeNotFound();

public record ServerError(string Text);

[GenerateOneOf]
public partial class Errors : OneOfBase<WrongFormat, ShortCodeNotFound, ServerError> { }
