using System.Collections.Concurrent;
using System.Net.Http.Json;
using Santander_Hacker_News_Service.Models;

namespace Santander_Hacker_News_Service.Services;

/// <summary>
/// Service for retrieving stories from the Hacker News API with caching and rate limiting
/// </summary>
public class HackerNewsService : IHackerNewsService
{
    private const string BestStoriesUrl = "https://hacker-news.firebaseio.com/v0/beststories.json";
    private const string ItemUrl = "https://hacker-news.firebaseio.com/v0/item/{0}.json";
    private const int CacheDurationMinutes = 5;
    private const int MaxConcurrentRequests = 10; // Limit concurrent requests to avoid overwhelming HN API

    private readonly HttpClient _httpClient;
    private readonly ILogger<HackerNewsService> _logger;
    private readonly SemaphoreSlim _semaphore;
    private readonly ConcurrentDictionary<int, (StoryResponse Story, DateTime CachedAt)> _cache;

    public HackerNewsService(HttpClient httpClient, ILogger<HackerNewsService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _semaphore = new SemaphoreSlim(MaxConcurrentRequests, MaxConcurrentRequests);
        _cache = new ConcurrentDictionary<int, (StoryResponse, DateTime)>();
    }

    public async Task<List<StoryResponse>> GetBestStoriesAsync(int count, CancellationToken cancellationToken = default)
    {
        if (count <= 0)
        {
            throw new ArgumentException("Count must be greater than 0", nameof(count));
        }

        try
        {
            // Fetch best story IDs
            var storyIds = await FetchBestStoryIdsAsync(cancellationToken);

            if (storyIds == null || storyIds.Count == 0)
            {
                _logger.LogWarning("No story IDs retrieved from Hacker News API");
                return new List<StoryResponse>();
            }

            // Take only the requested count
            var topStoryIds = storyIds.Take(count).ToList();

            // Fetch story details in parallel with concurrency limit
            var stories = await FetchStoriesDetailsAsync(topStoryIds, cancellationToken);

            // Sort by score descending
            return stories
                .OrderByDescending(s => s.Score)
                .ToList();
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Operation cancelled while fetching best stories");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching best stories");
            throw;
        }
    }

    private async Task<List<int>> FetchBestStoryIdsAsync(CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.GetAsync(BestStoriesUrl, cancellationToken);
            response.EnsureSuccessStatusCode();

            var ids = await response.Content.ReadFromJsonAsync<List<int>>(cancellationToken);
            return ids ?? new List<int>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching best story IDs");
            throw;
        }
    }

    private async Task<List<StoryResponse>> FetchStoriesDetailsAsync(List<int> storyIds, CancellationToken cancellationToken)
    {
        var stories = new ConcurrentBag<StoryResponse>();
        var tasks = new List<Task>();

        foreach (var storyId in storyIds)
        {
            tasks.Add(FetchAndCacheStoryAsync(storyId, stories, cancellationToken));
        }

        await Task.WhenAll(tasks);
        return stories.ToList();
    }

    private async Task FetchAndCacheStoryAsync(int storyId, ConcurrentBag<StoryResponse> stories, CancellationToken cancellationToken)
    {
        // Check cache first
        if (_cache.TryGetValue(storyId, out var cachedItem))
        {
            if (DateTime.UtcNow - cachedItem.CachedAt < TimeSpan.FromMinutes(CacheDurationMinutes))
            {
                stories.Add(cachedItem.Story);
                return;
            }
            else
            {
                // Remove expired cache entry
                _cache.TryRemove(storyId, out _);
            }
        }

        // Use semaphore to limit concurrent requests
        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            var storyResponse = await FetchStoryDetailAsync(storyId, cancellationToken);
            if (storyResponse != null)
            {
                // Cache the result
                _cache.AddOrUpdate(storyId, (storyResponse, DateTime.UtcNow), (_, __) => (storyResponse, DateTime.UtcNow));
                stories.Add(storyResponse);
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task<StoryResponse?> FetchStoryDetailAsync(int storyId, CancellationToken cancellationToken)
    {
        try
        {
            var url = string.Format(ItemUrl, storyId);
            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var item = await response.Content.ReadFromJsonAsync<HackerNewsItem>(cancellationToken);

            if (item == null || item.Deleted || item.Dead)
            {
                return null;
            }

            return new StoryResponse
            {
                Title = item.Title,
                Uri = item.Url,
                PostedBy = item.By,
                Time = UnixTimeStampToDateTime(item.Time),
                Score = item.Score,
                CommentCount = item.Descendants
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching story detail for ID {StoryId}", storyId);
            return null;
        }
    }

    private static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
    {
        var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp).ToUniversalTime();
        return dateTime;
    }
}