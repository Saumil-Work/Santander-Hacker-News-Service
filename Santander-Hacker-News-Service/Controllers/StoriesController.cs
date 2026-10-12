using Santander_Hacker_News_Service.Services;
using Microsoft.AspNetCore.Mvc;

namespace Santander_Hacker_News_Service.Controllers;

/// <summary>
/// API endpoints for retrieving Hacker News stories
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class StoriesController : ControllerBase
{
    private readonly IHackerNewsService _hackerNewsService;
    private readonly ILogger<StoriesController> _logger;

    public StoriesController(IHackerNewsService hackerNewsService, ILogger<StoriesController> logger)
    {
        _hackerNewsService = hackerNewsService;
        _logger = logger;
    }

    /// <summary>
    /// Gets the top n best stories from Hacker News
    /// </summary>
    /// <param name="n">Number of top stories to retrieve (1-500)</param>
    /// <returns>Array of stories sorted by score in descending order</returns>
    /// <response code="200">Successfully retrieved stories</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="500">Server error</response>
    [HttpGet("best")]
    [ProducesResponseType(typeof(List<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetBestStories([FromQuery] int n = 10, CancellationToken cancellationToken = default)
    {
        try
        {
            if (n < 1)
            {
                return BadRequest(new { error = "Parameter 'n' must be at least 1" });
            }

            if (n > 500)
            {
                return BadRequest(new { error = "Parameter 'n' must not exceed 500" });
            }

            _logger.LogInformation("Fetching top {N} stories", n);

            var stories = await _hackerNewsService.GetBestStoriesAsync(n, cancellationToken);

            return Ok(stories);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Request cancelled");
            return StatusCode(StatusCodes.Status408RequestTimeout, new { error = "Request timed out" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving best stories");
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = "An error occurred while retrieving stories" });
        }
    }
}