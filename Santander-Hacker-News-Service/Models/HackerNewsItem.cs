namespace Santander_Hacker_News_Service.Models;

/// <summary>
/// Represents a raw item from the Hacker News API
/// </summary>
public class HackerNewsItem
{
    public int Id { get; set; }
    public string? Type { get; set; }
    public string? By { get; set; }
    public long Time { get; set; }
    public string? Text { get; set; }
    public bool Dead { get; set; }
    public bool Deleted { get; set; }
    public string? Title { get; set; }
    public string? Url { get; set; }
    public int Score { get; set; }
    public int Descendants { get; set; }
}