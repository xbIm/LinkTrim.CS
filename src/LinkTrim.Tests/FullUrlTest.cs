using LinkTrim.Web.Models;

namespace LinkTrim.Tests;

public class FullUrlTest
{
    [Fact]
    public void Create_ValidUrl_ReturnsFullUrlObject()
    {
        // Arrange
        const string validUrl = "https://www.example.com/test-url   ";
        const string expectedTrimmedUrl = "https://www.example.com/test-url";

        // Act
        var result = FullUrl.Create(validUrl);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedTrimmedUrl, result.Success.Value);
    }

    [Fact]
    public void Create_InvalidUrl_ReturnsError()
    {
        // Arrange
        const string invalidUrl = "not_a_valid_url";

        // Act
        var result = FullUrl.Create(invalidUrl);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Invalid URL", result.Failure.AsT0.Text);
    }
}