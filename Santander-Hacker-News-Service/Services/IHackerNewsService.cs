using Santander_Hacker_News_Service.Models;

namespace Santander_Hacker_News_Service.Services;

/// <summary>
/// Interface for interacting with the Hacker News API
/// </summary>
public interface IHackerNewsService
{
    /// <summary>
    /// Gets the top n best stories by score
    /// </summary>
    /// <param name="count">Number of top stories to retrieve</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of top stories ordered by score descending</returns>
    Task<List<StoryResponse>> GetBestStoriesAsync(int count, CancellationToken cancellationToken = default);
}