namespace LinkTrim.Tests.IntegrationTests;

public class MvcIntegrationTests(IntegrationTestFactory<Program> factory) : IClassFixture<IntegrationTestFactory<Program>>
{
    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetIndex_Successfully()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/");

        response.EnsureSuccessStatusCode();
        Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType!.ToString());
    }
}