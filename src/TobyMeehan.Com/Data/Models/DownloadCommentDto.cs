namespace TobyMeehan.Com.Data.Models;

public class DownloadCommentDto
{
    public Guid DownloadId { get; set; }
    public Guid CommentId { get; set; }

    public DownloadDto? Download { get; set; }
    public CommentDto? Comment { get; set; }
}