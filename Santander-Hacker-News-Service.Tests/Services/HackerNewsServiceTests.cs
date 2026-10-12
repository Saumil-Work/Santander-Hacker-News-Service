using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Santander_Hacker_News_Service.Models;
using Santander_Hacker_News_Service.Services;
using System.Net;
using System.Text;
using Xunit;

namespace Santander_Hacker_News_Service.Tests.Services;

public class HackerNewsServiceTests
{
    private readonly Mock<ILogger<HackerNewsService>> _loggerMock;
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly HttpClient _httpClient;

    public HackerNewsServiceTests()
    {
        _loggerMock = new Mock<ILogger<HackerNewsService>>();
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri("https://hacker-news.firebaseio.com")
        };
    }

    [Fact]
    public async Task GetBestStoriesAsync_WithValidCount_ReturnsStoriesSortedByScore()
    {
        // Arrange
        var bestStoryIds = new[] { 1, 2, 3, 4, 5 };
        var bestStoriesJson = System.Text.Json.JsonSerializer.Serialize(bestStoryIds.ToList());

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(r => r.RequestUri!.ToString().Contains("beststories")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(bestStoriesJson, System.Text.Encoding.UTF8, "application/json")
            });

        var storyResponses = new List<HackerNewsItem>
        {
            new() { Id = 1, Title = "Story 1", By = "user1", Time = 1000, Score = 100, Descendants = 5, Url = "https://example1.com" },
            new() { Id = 2, Title = "Story 2", By = "user2", Time = 2000, Score = 50, Descendants = 3, Url = "https://example2.com" },
            new() { Id = 3, Title = "Story 3", By = "user3", Time = 3000, Score = 150, Descendants = 10, Url = "https://example3.com" },
            new() { Id = 4, Title = "Story 4", By = "user4", Time = 4000, Score = 75, Descendants = 7, Url = "https://example4.com" },
            new() { Id = 5, Title = "Story 5", By = "user5", Time = 5000, Score = 120, Descendants = 8, Url = "https://example5.com" }
        };

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(r => r.RequestUri!.ToString().Contains("/item/")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync((HttpRequestMessage request, CancellationToken ct) =>
            {
                var storyId = int.Parse(request.RequestUri!.ToString().Split('/')[5].Replace(".json", ""));
                var story = storyResponses.FirstOrDefault(s => s.Id == storyId);
                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(story), System.Text.Encoding.UTF8, "application/json")
                };
            });

        var service = new HackerNewsService(_httpClient, _loggerMock.Object);

        // Act
        var result = await service.GetBestStoriesAsync(5);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(5, result.Count);
        
        // Verify sorted by score descending
        Assert.Equal(150, result[0].Score); // Story 3
        Assert.Equal(120, result[1].Score); // Story 5
        Assert.Equal(100, result[2].Score); // Story 1
        Assert.Equal(75, result[3].Score);  // Story 4
        Assert.Equal(50, result[4].Score);  // Story 2
    }

    [Fact]
    public async Task GetBestStoriesAsync_WithCountLessThanOne_ThrowsArgumentException()
    {
        // Arrange
        var service = new HackerNewsService(_httpClient, _loggerMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => service.GetBestStoriesAsync(0));
    }

    [Fact]
    public async Task GetBestStoriesAsync_WithNegativeCount_ThrowsArgumentException()
    {
        // Arrange
        var service = new HackerNewsService(_httpClient, _loggerMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => service.GetBestStoriesAsync(-5));
    }

    [Fact]
    public async Task GetBestStoriesAsync_WithDeletedStory_ExcludesFromResults()
    {
        // Arrange
        var bestStoryIds = new[] { 1, 2, 3 };
        var bestStoriesJson = System.Text.Json.JsonSerializer.Serialize(bestStoryIds.ToList());

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(r => r.RequestUri!.ToString().Contains("beststories")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(bestStoriesJson, Encoding.UTF8, "application/json")
            });

        var storyResponses = new Dictionary<int, HackerNewsItem>
        {
            { 1, new() { Id = 1, Title = "Story 1", By = "user1", Time = 1000, Score = 100, Descendants = 5, Url = "https://example1.com" } },
            { 2, new() { Id = 2, Title = null, By = null, Deleted = true } }, // Deleted story
            { 3, new() { Id = 3, Title = "Story 3", By = "user3", Time = 3000, Score = 150, Descendants = 10, Url = "https://example3.com" } }
        };

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(r => r.RequestUri!.ToString().Contains("/item/")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync((HttpRequestMessage request, CancellationToken ct) =>
            {
                var storyId = int.Parse(request.RequestUri!.ToString().Split('/')[5].Replace(".json", ""));
                var story = storyResponses[storyId];
                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(story), System.Text.Encoding.UTF8, "application/json")
                };
            });

        var service = new HackerNewsService(_httpClient, _loggerMock.Object);

        // Act
        var result = await service.GetBestStoriesAsync(3);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.DoesNotContain(result, s => s.Title == null);
    }

    [Fact]
    public async Task GetBestStoriesAsync_WithDeadStory_ExcludesFromResults()
    {
        // Arrange
        var bestStoryIds = new[] { 1, 2 };
         var bestStoriesJson = System.Text.Json.JsonSerializer.Serialize(bestStoryIds.ToList());

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(r => r.RequestUri!.ToString().Contains("beststories")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(bestStoriesJson, System.Text.Encoding.UTF8, "application/json")
            });

        var storyResponses = new Dictionary<int, HackerNewsItem>
        {
            { 1, new() { Id = 1, Title = "Story 1", By = "user1", Time = 1000, Score = 100, Descendants = 5, Url = "https://example1.com" } },
            { 2, new() { Id = 2, Title = "Dead Story", By = "user2", Dead = true, Time = 2000, Score = 50, Descendants = 0, Url = "https://example2.com" } }
        };

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(r => r.RequestUri!.ToString().Contains("/item/")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync((HttpRequestMessage request, CancellationToken ct) =>
            {
                var storyId = int.Parse(request.RequestUri!.ToString().Split('/')[5].Replace(".json", ""));
                var story = storyResponses[storyId];
                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(story), System.Text.Encoding.UTF8, "application/json")
                };
            });

        var service = new HackerNewsService(_httpClient, _loggerMock.Object);

        // Act
        var result = await service.GetBestStoriesAsync(2);

        // Assert
        Assert.Single(result);
        Assert.Equal("Story 1", result[0].Title);
    }

    [Fact]
    public async Task GetBestStoriesAsync_WithEmptyBestStoriesList_ReturnsEmptyList()
    {
        // Arrange
        var bestStoriesJson = System.Text.Json.JsonSerializer.Serialize(new List<int>());

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(r => r.RequestUri!.ToString().Contains("beststories")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(bestStoriesJson, System.Text.Encoding.UTF8, "application/json")
            });

        var service = new HackerNewsService(_httpClient, _loggerMock.Object);

        // Act
        var result = await service.GetBestStoriesAsync(10);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetBestStoriesAsync_WithHttpException_LogsErrorAndThrows()
    {
        // Arrange
        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(r => r.RequestUri!.ToString().Contains("beststories")),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Connection failed"));

        var service = new HackerNewsService(_httpClient, _loggerMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => service.GetBestStoriesAsync(10));
    }

    [Fact]
    public async Task GetBestStoriesAsync_WithCancellation_ThrowsOperationCanceledException()
    {
        // Arrange
        // Create a cancelled token BEFORE calling the service
        var cts = new CancellationTokenSource();
        cts.CancelAfter(10); // Cancel after 10ms to let the operation start

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Returns((HttpRequestMessage request, CancellationToken ct) =>
            {
                // Return a task that will be canceled
                return Task.Delay(5000, ct).ContinueWith(t => 
                    new HttpResponseMessage(HttpStatusCode.OK) 
                    { 
                        Content = new StringContent("[]", Encoding.UTF8, "application/json") 
                    }, 
                    cts.Token);
            });

        var service = new HackerNewsService(_httpClient, _loggerMock.Object);

        // Act & Assert
        // HttpClient throws TaskCanceledException (which inherits from OperationCanceledException) when cancelled
        await Assert.ThrowsAsync<TaskCanceledException>(() => 
            service.GetBestStoriesAsync(10, cts.Token));
    }

    [Fact]
    public async Task GetBestStoriesAsync_WithPartialResults_ReturnsFetchedStories()
    {
        // Arrange
        var bestStoryIds = new[] { 1, 2, 3 };
        var bestStoriesJson = System.Text.Json.JsonSerializer.Serialize(bestStoryIds.ToList());

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(r => r.RequestUri!.ToString().Contains("beststories")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(bestStoriesJson, Encoding.UTF8, "application/json")
            });

        var callCount = 0;
        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(r => r.RequestUri!.ToString().Contains("/item/")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync((HttpRequestMessage request, CancellationToken ct) =>
            {
                callCount++;
                if (callCount == 2) // Simulate failure on second story
                {
                    return new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.NotFound
                    };
                }

                var storyId = int.Parse(request.RequestUri!.ToString().Split('/')[5].Replace(".json", ""));
                var story = new HackerNewsItem
                {
                    Id = storyId,
                    Title = $"Story {storyId}",
                    By = $"user{storyId}",
                    Time = storyId * 1000,
                    Score = storyId * 100,
                    Descendants = storyId * 5,
                    Url = $"https://example{storyId}.com"
                };
                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(story), Encoding.UTF8, "application/json")
                };
            });

        var service = new HackerNewsService(_httpClient, _loggerMock.Object);

        // Act
        var result = await service.GetBestStoriesAsync(3);

        // Assert
        Assert.Equal(2, result.Count); // Only 2 successful stories
    }
}