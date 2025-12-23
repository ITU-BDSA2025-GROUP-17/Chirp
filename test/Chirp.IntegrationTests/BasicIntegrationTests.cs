using Microsoft.AspNetCore.Mvc.Testing;

namespace Chirp.IntegrationTests;

public class BasicIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public BasicIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PublicPage_ReturnsSuccessStatusCode()
    {
        // Arrange - ingen setup nødvendig

        // Act - lav HTTP request til public page
        var response = await _client.GetAsync("/");

        // Assert - tjek at det lykkedes
        response.EnsureSuccessStatusCode();
        Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType?.ToString());
    }

    [Fact]
    public async Task PublicPage_ContainsCheepWord()
    {
        // Arrange

        // Act
        var response = await _client.GetAsync("/");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Contains("Chirp", content);
    }
}
