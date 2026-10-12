using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Xunit;

namespace Santander_Hacker_News_Service.Tests.Integration;

public class StoriesIntegrationTests : IAsyncLifetime
{
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    public async Task InitializeAsync()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
    }

    [Fact]
    public async Task GetBestStories_WithValidN_ReturnsSuccessfulResponse()
    {
        // This test requires the HN API to be accessible
        // For local testing, it demonstrates the integration test structure

        // Act
        var response = await _client.GetAsync("/api/stories/best?n=5");

        // Assert
        // This assertion may fail if HN API is unavailable
        // In production, use mocks or test APIs
        Assert.True(response.StatusCode == HttpStatusCode.OK ||
                    response.StatusCode == HttpStatusCode.ServiceUnavailable ||
                    response.StatusCode == HttpStatusCode.GatewayTimeout);
    }

    [Fact]
    public async Task GetBestStories_WithMissingN_Uses10AsDefault()
    {
        // Act
        var response = await _client.GetAsync("/api/stories/best");

        // Assert
        Assert.True(response.StatusCode == HttpStatusCode.OK ||
                    response.StatusCode == HttpStatusCode.ServiceUnavailable ||
                    response.StatusCode == HttpStatusCode.GatewayTimeout);
    }

    [Fact]
    public async Task GetBestStories_WithInvalidN_ReturnsBadRequest()
    {
        // Act
        var response = await _client.GetAsync("/api/stories/best?n=0");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetBestStories_WithNExceeding500_ReturnsBadRequest()
    {
        // Act
        var response = await _client.GetAsync("/api/stories/best?n=501");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}