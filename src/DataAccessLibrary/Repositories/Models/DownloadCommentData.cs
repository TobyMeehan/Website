namespace TobyMeehan.Com.Data.Repositories.Models;

public class DownloadCommentData
{
    public string Id { get; set; }
    public string AuthorId { get; set; }
    public string DownloadId { get; set; }
    public string Content { get; set; }
    public DateTimeOffset SentAt { get; set; }
    public DateTimeOffset? EditedAt { get; set; }
}