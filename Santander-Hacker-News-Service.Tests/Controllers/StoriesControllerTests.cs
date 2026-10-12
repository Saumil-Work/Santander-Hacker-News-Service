using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Santander_Hacker_News_Service.Controllers;
using Santander_Hacker_News_Service.Models;
using Santander_Hacker_News_Service.Services;
using Xunit;

namespace Santander_Hacker_News_Service.Tests.Controllers;

public class StoriesControllerTests
{
    private readonly Mock<IHackerNewsService> _hackerNewsServiceMock;
    private readonly Mock<ILogger<StoriesController>> _loggerMock;
    private readonly StoriesController _controller;

    public StoriesControllerTests()
    {
        _hackerNewsServiceMock = new Mock<IHackerNewsService>();
        _loggerMock = new Mock<ILogger<StoriesController>>();
        _controller = new StoriesController(_hackerNewsServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetBestStories_WithValidN_ReturnsOkWithStories()
    {
        // Arrange
        var stories = new List<StoryResponse>
        {
            new()
            {
                Title = "Test Story 1",
                Uri = "https://example.com/1",
                PostedBy = "user1",
                Time = DateTime.UtcNow,
                Score = 100,
                CommentCount = 5
            },
            new()
            {
                Title = "Test Story 2",
                Uri = "https://example.com/2",
                PostedBy = "user2",
                Time = DateTime.UtcNow,
                Score = 50,
                CommentCount = 3
            }
        };

        _hackerNewsServiceMock
            .Setup(s => s.GetBestStoriesAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(stories);

        // Act
        var result = await _controller.GetBestStories(10);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);

        var returnedStories = Assert.IsType<List<StoryResponse>>(okResult.Value);
        Assert.Equal(2, returnedStories.Count);
        Assert.Equal("Test Story 1", returnedStories[0].Title);
        Assert.Equal("Test Story 2", returnedStories[1].Title);

        _hackerNewsServiceMock.Verify(
            s => s.GetBestStoriesAsync(10, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetBestStories_WithDefaultN_Uses10AsDefault()
    {
        // Arrange
        var stories = new List<StoryResponse>();
        _hackerNewsServiceMock
            .Setup(s => s.GetBestStoriesAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(stories);

        // Act
        await _controller.GetBestStories();

        // Assert
        _hackerNewsServiceMock.Verify(
            s => s.GetBestStoriesAsync(10, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetBestStories_WithNEqualToZero_ReturnsBadRequest()
    {
        // Act
        var result = await _controller.GetBestStories(0);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.NotNull(badRequestResult.Value);
    }

    [Fact]
    public async Task GetBestStories_WithNLessThanZero_ReturnsBadRequest()
    {
        // Act
        var result = await _controller.GetBestStories(-5);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
    }

    [Fact]
    public async Task GetBestStories_WithNGreaterThan500_ReturnsBadRequest()
    {
        // Act
        var result = await _controller.GetBestStories(501);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.NotNull(badRequestResult.Value);
    }

    [Fact]
    public async Task GetBestStories_WithNEqualTo500_ReturnsOk()
    {
        // Arrange
        var stories = new List<StoryResponse>();
        _hackerNewsServiceMock
            .Setup(s => s.GetBestStoriesAsync(500, It.IsAny<CancellationToken>()))
            .ReturnsAsync(stories);

        // Act
        var result = await _controller.GetBestStories(500);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetBestStories_WithNEqualTo1_ReturnsOk()
    {
        // Arrange
        var stories = new List<StoryResponse>
        {
            new() { Title = "Story", Uri = "https://example.com", PostedBy = "user", Score = 100, CommentCount = 5 }
        };
        _hackerNewsServiceMock
            .Setup(s => s.GetBestStoriesAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(stories);

        // Act
        var result = await _controller.GetBestStories(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }

    [Fact]
    public async Task GetBestStories_WhenOperationCancelled_ReturnsRequestTimeout()
    {
        // Arrange
        _hackerNewsServiceMock
            .Setup(s => s.GetBestStoriesAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act
        var result = await _controller.GetBestStories(10);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(408, statusResult.StatusCode);
        Assert.NotNull(statusResult.Value);
    }

    [Fact]
    public async Task GetBestStories_WhenServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        _hackerNewsServiceMock
            .Setup(s => s.GetBestStoriesAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var result = await _controller.GetBestStories(10);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusResult.StatusCode);
        Assert.NotNull(statusResult.Value);
    }

    [Fact]
    public async Task GetBestStories_WithEmptyResults_ReturnsOkWithEmptyArray()
    {
        // Arrange
        var stories = new List<StoryResponse>();
        _hackerNewsServiceMock
            .Setup(s => s.GetBestStoriesAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(stories);

        // Act
        var result = await _controller.GetBestStories(10);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedStories = Assert.IsType<List<StoryResponse>>(okResult.Value);
        Assert.Empty(returnedStories);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(50)]
    [InlineData(100)]
    [InlineData(500)]
    public async Task GetBestStories_WithValidNValues_ReturnsOk(int n)
    {
        // Arrange
        var stories = new List<StoryResponse>();
        _hackerNewsServiceMock
            .Setup(s => s.GetBestStoriesAsync(n, It.IsAny<CancellationToken>()))
            .ReturnsAsync(stories);

        // Act
        var result = await _controller.GetBestStories(n);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    [InlineData(0)]
    public async Task GetBestStories_WithInvalidNValues_ReturnsBadRequest(int n)
    {
        // Act
        var result = await _controller.GetBestStories(n);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Theory]
    [InlineData(501)]
    [InlineData(1000)]
    [InlineData(10000)]
    public async Task GetBestStories_WithNExceeding500_ReturnsBadRequest(int n)
    {
        // Act
        var result = await _controller.GetBestStories(n);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
}