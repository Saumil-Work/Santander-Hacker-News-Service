namespace Santander_Hacker_News_Service.Models;

/// <summary>
/// Represents a Hacker News story to be returned by our API
/// </summary>
public class StoryResponse
{
    public string? Title { get; set; }
    public string? Uri { get; set; }
    public string? PostedBy { get; set; }
    public DateTime Time { get; set; }
    public int Score { get; set; }
    public int CommentCount { get; set; }
}