using Microsoft.AspNetCore.Mvc.Testing;

namespace Chirp.IntegrationTests;

public class BasicIntegrationTests : IClassFixture<ChirpWebApplicationFactory>
{
    private readonly ChirpWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public BasicIntegrationTests(ChirpWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PublicPage_ReturnsSuccessStatusCode()
    {
        // Arrange - no setup needed

        // Act - make HTTP request to public page
        var response = await _client.GetAsync("/");

        response.EnsureSuccessStatusCode();
        Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType?.ToString());
    }

    [Fact]
    public async Task PublicPage_ContainsCheepWord()
    {
        // Arrange - no setup needed

        // Act
        var response = await _client.GetAsync("/");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("Chirp", content);
    }
}
